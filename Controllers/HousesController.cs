using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HouseRentals.Data;
using HouseRentals.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HouseRentals.Controllers
{
    [Authorize]
    public class HousesController : Controller
    {
        private readonly HouseRentalsDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HousesController(HouseRentalsDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Houses
        public async Task<IActionResult> Index()
        {
            var houseRentalsDbContext = _context.Houses.Include(h => h.Owner);
            return View(await houseRentalsDbContext.ToListAsync());
        }

        //Rent
        [Authorize(Roles = "Tenant,Administrator")]
        [HttpPost]
        public async Task<IActionResult> Rent(int id)
        {
            var house = await _context.Houses.FindAsync(id);

            if (house == null)
                return NotFound();

            if (!house.Available)
                return BadRequest("Къщата вече е заета");

            house.Available = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Houses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.Houses
                .Include(h => h.Owner)
                .FirstOrDefaultAsync(m => m.HouseId == id);
            if (house == null)
            {
                return NotFound();
            }

            return View(house);
        }

        // GET: Houses/Create
        [Authorize(Roles = "Owner,Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Houses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Administrator")]
        public async Task<IActionResult> Create(
            [Bind("Address,Description,Price_Per_Month")] House house)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                var owner = await _context.Owners
                    .FirstOrDefaultAsync(o => o.ApplicationUserId == userId);

                if (owner == null)
                {
                    return Content("Owner not found");
                }

                house.OwnerId = owner.OwnerId;
                house.Available = true;

                _context.Houses.Add(house);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else if(!ModelState.IsValid)
            {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
            }

            return View(house);
        }

        // GET: Houses/Edit/5
        [Authorize(Roles = "Owner,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.Houses.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }
            return View(house);
        }

        // POST: Houses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Administrator")]
        public async Task<IActionResult> Edit(int id,
            [Bind("HouseId,Address,Description,Price_Per_Month,Available")] House house)
        {
            if (id != house.HouseId)
                return NotFound();

            var existingHouse = await _context.Houses.AsNoTracking()
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (existingHouse == null)
                return NotFound();

            house.OwnerId = existingHouse.OwnerId;

            if (ModelState.IsValid)
            {
                _context.Update(house);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(house);
        }

        // GET: Houses/Delete/5
        [Authorize(Roles = "Owner,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var house = await _context.Houses
                .Include(h => h.Owner)
                .FirstOrDefaultAsync(m => m.HouseId == id);
            if (house == null)
            {
                return NotFound();
            }

            return View(house);
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house != null)
            {
                _context.Houses.Remove(house);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HouseExists(int id)
        {
            return _context.Houses.Any(e => e.HouseId == id);
        }
    }
}
