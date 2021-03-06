﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment1_Salesboard.Data;
using Assignment1_Salesboard.Models;

namespace Assignment1_Salesboard.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public SalesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUserName(User);
            if (user == "admin@cdu.com")
            {
                ViewBag.errorMessage = "Admin only has access to Admin tab.";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            return View(await _context.Sales.ToListAsync());
        }

        //Get my sales
        public IActionResult MySales()
        {
            var seller = _userManager.GetUserName(User);

            if (seller == null)
            {
                ViewBag.errorMessage = "You are currently not logged in, please log in to proceed!";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (seller == "admin@cdu.com")
            {
                ViewBag.errorMessage = "Admin only has access to Admin tab";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            var sales = _context.Sales
                .Where(s => s.Seller == seller);

            return View("Index", sales);
        }

        // GET: Get my Purchase
        public ActionResult MyPurchases()
        {
            var buyer = _userManager.GetUserName(User);
            if (buyer == "admin@cdu.com")
            {
                ViewBag.errorMessage = "Admin only has access to Admin tab";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            var sales = _context.Sales
                .Where(m => m.Buyer == buyer);

            return View("MyPurchases", sales);
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sales == null)
            {
                return NotFound();
            }

            return View(sales);
        }

        // GET: Sales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Item,Buyer,Quantity")] Sales sales)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sales);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sales);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await _context.Sales.FindAsync(id);
            if (sales == null)
            {
                return NotFound();
            }
            return View(sales);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Item,Buyer,Quantity")] Sales sales)
        {
            if (id != sales.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesExists(sales.Id))
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
            return View(sales);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var sales = await _context.Sales
                .FirstOrDefaultAsync(m => m.Id == id);
            var user = _userManager.GetUserName(User);
            if (user != sales.Seller)
            {
                ViewBag.errorMessage = "You can't delete the item that you have purchased!";
                return View("Views/Home/Error.cshtml", ViewBag.errorMessage);
            }

            if (id == null)
            {
                return NotFound();
            }

            if (sales == null)
            {
                return NotFound();
            }

            return View(sales);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sales = await _context.Sales.FindAsync(id);
            _context.Sales.Remove(sales);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
