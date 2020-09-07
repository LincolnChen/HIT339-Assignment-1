using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        public ItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
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

            var items = await _context.Items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
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
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: Items/Purchase/5
        [HttpPost, ActionName("Purchase")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseConfirmed([Bind("Item,Quantity")] Sales sales)
        {
            // get the buyer
            var buyer = _userManager.GetUserName(User);
            sales.Buyer = buyer;

            // make the sale
            _context.Add(sales);

            // find the item
            var items = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == sales.Item);

            if (items == null)
            {
                return NotFound();
            }

            if (items.Quantity == 0)
            {
                ViewBag.errorMessage = "Sorry, we are currently running out of stock.";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (items.Quantity <= sales.Quantity)
            {
                ViewBag.errorMessage = "Please check your quantity of item/s before purchsaing";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            // update the quantity
            items.Quantity -= sales.Quantity;
            _context.Update(items);

            // Save the changes
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ItemsExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
