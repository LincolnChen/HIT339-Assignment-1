using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment1_Salesboard.Data;
using Assignment1_Salesboard.Models;

namespace Assignment1_Salesboard
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _session;


        public ItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor session)
        {
            _userManager = userManager;
            _context = context;
            _session = session;

        }

        // GET: Items
        public async Task<IActionResult> Index(string searchString)
        {
            var items = from m in _context.Items
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.ItemName.Contains(searchString));
            }

            return View(await items.ToListAsync());
        }


        // GET: Items/myItems
        public ActionResult MyItems()
        {
            var seller = _userManager.GetUserName(User);
            var items = _context.Items
                .Where(m => m.Seller == seller);
            return View("Index", items);

        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // GET: Items/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemName,ItemDescription,Price,Quantity,Seller")] Items items)
        {
            if (ModelState.IsValid)
            {
                // get the seller
                var seller = _userManager.GetUserName(User);
                items.Seller = seller;
                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(items);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userManager.GetUserName(User);
            if (user == null)
            {
                ViewBag.errorMessage = "You are currently not logged in. Please log in to edit items!";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            var items = await _context.Items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            if (user != items.Seller)
            {
                ViewBag.errorMessage = "You don't have permission to edit other seller's item. Please log in the user(Seller) to edit!";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }
            return View(items);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,ItemDescription,Price,Quantity,Seller")] Items items)
        {
            if (id != items.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(items);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemsExists(items.Id))
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
            return View(items);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            var AccessDenied = _userManager.GetUserName(User);
            if (items.Seller != AccessDenied)
            {
                ViewBag.errorMessage = "You don't have permission to delete other seller's item. Please log in the user(Seller) to delete!";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var items = await _context.Items.FindAsync(id);
            _context.Items.Remove(items);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Items/Purchase/5
        public async Task<IActionResult> Purchase(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            var user = _userManager.GetUserName(User);
            if (user == null)
            {
                ViewBag.errorMessage = "You must be logged in to add items to cart. Please log in!";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            var seller = items.Seller;

            if (user == seller)
            {
                ViewBag.errorMessage = "You are the SELLER, You can't purchase your own item.";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (items.Quantity <= 0)
            {
                ViewBag.errorMessage = "Sorry, we are currently running out of stock. Please check back later";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: Items/Purchase/5
        [HttpPost, ActionName("Purchase")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseConfirmed([Bind("Item,Quantity,Price,Seller")] ShoppingCart shoppingCart, Sales sales)
        {
            // get or create a cart id
            string cartId = _session.HttpContext.Session.GetString("cartId");

            if (string.IsNullOrEmpty(cartId) == true) cartId = Guid.NewGuid().ToString();

            var items = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == sales.Item);

            if (sales.Quantity == 0)
            {
                ViewBag.errorMessage = "Sorry, we are currently running out of stock. Please check back later";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (items.Quantity < sales.Quantity)
            {
                ViewBag.errorMessage = "Sorry, we don't have enough stock for you order. Please check back later";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }


            //if (items.Quantity <= sales.Quantity)
            //{
            //    ViewBag.errorMessage = "Please check your quantity of item before adding to cart";
            //    return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            //}



            // use the cart id
            shoppingCart.CartId = cartId.ToString();

            var item = await _context.Items
                    .FirstOrDefaultAsync(m => m.Id == shoppingCart.Item);

            // Calculate the total amount of price on Shopping Cart page
            var total = shoppingCart.Quantity * item.Price;


            // make the sale
            _context.Add(shoppingCart);

            // add to cart
            var checkCount = _session.HttpContext.Session.GetInt32("cartCount");
            int cartCount = checkCount == null ? 0 : (int)checkCount;
            _session.HttpContext.Session.SetString("cartId", cartId.ToString());
            _session.HttpContext.Session.SetInt32("cartCount", ++cartCount);

            // Save the changes
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        //// POST: ItemsController/AddToCart/5
        //[HttpPost, ActionName("AddToCart")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddToCart([Bind("Item,Quantity,Price,Seller")] ShoppingCart shoppingCart)
        //{

        //    // get the buyer
        //    var seller = _userManager.GetUserName(User);
        //    shoppingCart.Seller = seller;

        //    // make the add
        //    _context.Add(shoppingCart);

        //    // find the item
        //    var items = await _context.Items
        //        .FirstOrDefaultAsync(i => i.Id == shoppingCart.Item);

        //    if (items == null)
        //    {
        //        return NotFound();
        //    }

        //    if (items.Quantity == 0)
        //    {
        //        ViewBag.errorMessage = "Sorry, we are currently running out of stock.";
        //        return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
        //    }

        //    if (items.Quantity < 0)
        //    {
        //        ViewBag.errorMessage = "Please check your quantity of item/s before purchsaing";
        //        return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
        //    }


        //    // Save the changes
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}


        private bool ItemsExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
