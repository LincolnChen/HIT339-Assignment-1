using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment1_Salesboard.Data;
using Assignment1_Salesboard.Models;

namespace Assignment1_Salesboard.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _session;


        public ShoppingCartsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor session)
        {
            _userManager = userManager;
            _context = context;
            _session = session;

        }

        // GET: ShoppingCarts
        public async Task<IActionResult> Index()
        {
      
            var cartId = _session.HttpContext.Session.GetString("cartId");
            var carts = _context.ShoppingCart
                .Where(c => c.CartId == cartId);

            return View(await carts.ToListAsync());
        }

        //// GET: Carts/myShoppingCarts
        //public ActionResult ShoppingCartItems()
        //{
        //    var seller = _userManager.GetUserName(User);
        //    var shoppingCart = _context.ShoppingCart
        //        .Where(m => m.Seller == seller);
        //    return View("Index", shoppingCart);
        //}


        // GET: ShoppingCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            var items = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == shoppingCart.Item);

            return View("Views/Items/Details.cshtml", items);

        }

        // GET: ShoppingCarts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShoppingCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CartId,Item,ItemImage,Quantity,Price,Seller")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCart.FindAsync(id);
            if (shoppingCart == null)
            {
                return NotFound();
            }
            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CartId,Item,ItemImage,Quantity,Price,Seller")] ShoppingCart shoppingCart)
        {
            if (id != shoppingCart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingCartExists(shoppingCart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.ShoppingCart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCart = await _context.ShoppingCart.FindAsync(id);
            _context.ShoppingCart.Remove(shoppingCart);
            await _context.SaveChangesAsync();
            // add to cart
            var checkCount = _session.HttpContext.Session.GetInt32("cartCount");
            int cartCount = checkCount == null ? 0 : (int)checkCount;
            _session.HttpContext.Session.SetInt32("cartCount", --cartCount);
            return RedirectToAction(nameof(Index));
        }

        //Checkout
        [Authorize]
        public async Task<IActionResult> Checkout(Sales sales)
        {
            // get the cart id
            var cartId = _session.HttpContext.Session.GetString("cartId");

            // get the cart items
            var carts = _context.ShoppingCart
                .Where(c => c.CartId == cartId);

            if (cartId == null)
            {
                ViewBag.errorMessage = "Sorry, you haven't added item to your cart therefore you can't check out";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            // get the buyer
            var buyer = _userManager.GetUserName(User);

            // create the sales
            foreach (ShoppingCart shoppingCart in carts.ToList())
            {
                // find the item
                var item = await _context.Items
                    .FirstOrDefaultAsync(m => m.Id == shoppingCart.Item);

                var seller = shoppingCart.Seller;
                // update the quantity
                item.Quantity -= shoppingCart.Quantity;
                _context.Update(item);

                Sales sale = new Sales { Buyer = buyer, Seller = seller, Item = shoppingCart.Item, Quantity = shoppingCart.Quantity, TotalPrice = item.Price };
                _context.Update(sale);
            }

            // Save the changes
            await _context.SaveChangesAsync();

            // delete cart
            _session.HttpContext.Session.SetString("cartId", "");
            _session.HttpContext.Session.SetInt32("cartCount", 0);

            return View("Views/Home/ordersucessful.cshtml", ViewBag.ordersucessfulMessage);

        }

        private bool ShoppingCartExists(int id)
        {
            return _context.ShoppingCart.Any(e => e.Id == id);
        }
    }
}
