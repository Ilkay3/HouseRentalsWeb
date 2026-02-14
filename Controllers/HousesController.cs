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
            var houses = await _context.Houses
                .Include(h => h.Owner)
                .Include(h => h.City)
                .Include(h => h.Tenant)
                .ToListAsync();

            int? currentTenantId = null;

            if (User.IsInRole("Tenant"))
            {
                var userId = _userManager.GetUserId(User);

                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.ApplicationUserId == userId);

                if (tenant != null)
                {
                    currentTenantId = tenant.TenantId;
                }
            }

            ViewBag.CurrentTenantId = currentTenantId;

            return View(houses);
        }

        //Rent
        [Authorize(Roles = "Tenant,Administrator")]
        [HttpPost]
        public async Task<IActionResult> Rent(int id)
        {
            var house = await _context.Houses
                .Include(h => h.Rentals)
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (house == null)
                return NotFound();

            if (!house.Available)
                return BadRequest("Къщата вече е заета");

            var userId = _userManager.GetUserId(User);
            var tenant = await _context.Tenants
                .Include(t => t.Rentals)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == userId);

            if (tenant == null)
                return Content("Tenant not found");

            var activeRental = tenant.Rentals?.FirstOrDefault(r => r.IsActive);
            if (activeRental != null)
            {
                return BadRequest("Вече имате активен наем. Моля, първо освободете текущия имот.");
            }

            var rental = new Rental
            {
                HouseId = house.HouseId,
                TenantId = tenant.TenantId,
                RentDate = DateTime.Now,
                PriceAtRent = house.Price_Per_Month,
                IsActive = true,
                Notes = "Наето през уеб сайта"
            };

            _context.Rentals.Add(rental);

            // Актуализираме статуса на къщата
            house.Available = false;
            house.TenantId = tenant.TenantId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Успешно наехте имота!";
            return RedirectToAction(nameof(Index));
        }
        // End Rent
        [Authorize(Roles = "Tenant,Administrator")]
        [HttpPost]
        public async Task<IActionResult> StopRent(int id)
        {
            var house = await _context.Houses
                .Include(h => h.Rentals)
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (house == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.ApplicationUserId == userId);

            var activeRental = house.Rentals?
                .FirstOrDefault(r => r.IsActive && r.TenantId == (tenant?.TenantId ?? 0));

            if (activeRental == null && !User.IsInRole("Administrator"))
                return Forbid();

            if (User.IsInRole("Administrator") || User.IsInRole("Owner"))
            {
                activeRental = house.Rentals?.FirstOrDefault(r => r.IsActive);
                if (activeRental == null)
                    return BadRequest("Няма активен наем за този имот.");
            }

            // БИЗНЕС ЛОГИКА: Освобождаване
            activeRental!.ReleaseDate = DateTime.Now;
            activeRental.IsActive = false;

            // Изчисляваме сумата
            var days = (activeRental.ReleaseDate.Value - activeRental.RentDate).Days;
            if (days == 0) days = 1;

            double dailyPrice = activeRental.PriceAtRent / 30.0;
            activeRental.TotalAmount = days * dailyPrice;


            await _context.SaveChangesAsync();

            // Пренасочваме към плащане
            return RedirectToAction("Pay", new { rentalId = activeRental.RentalId });
        }

        [Authorize(Roles = "Tenant,Administrator")]
        public async Task<IActionResult> Pay(int rentalId)
        {
            var rental = await _context.Rentals
                .Include(r => r.House)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId);

            if (rental == null)
                return NotFound();

            return View(rental);
        }

        [HttpPost]
        [Authorize(Roles = "Tenant,Administrator")]
        public async Task<IActionResult> ConfirmPayment(int rentalId)
        {
            var rental = await _context.Rentals
                .Include(r => r.House)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId);

            if (rental == null)
                return NotFound();

            rental.IsPaid = true;

            rental.House!.Available = true;
            rental.House.TenantId = null;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Плащането беше успешно!";
            return RedirectToAction("Index");
        }


        // GET: Houses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var house = await _context.Houses
                .Include(h => h.Owner)
                    .ThenInclude(o => o.ApplicationUser)
                .Include(h => h.Tenant)
                    .ThenInclude(t => t.ApplicationUser)
                .Include(h => h.City)
                .Include(h => h.HouseAmenities)
                    .ThenInclude(ha => ha.Amenity)
                .FirstOrDefaultAsync(m => m.HouseId == id);

            if (house == null)
                return NotFound();

            return View(house);
        }

        // GET: Houses/MyHouses - само за собственик
        [Authorize(Roles = "Owner,Administrator")]
        public async Task<IActionResult> MyHouses()
        {
            var userId = _userManager.GetUserId(User);

            var owner = await _context.Owners
                .FirstOrDefaultAsync(o => o.ApplicationUserId == userId);

            if (owner == null && !User.IsInRole("Administrator"))
            {
                return NotFound("Нямате профил като собственик.");
            }

            if (User.IsInRole("Administrator"))
            {
                var allHouses = await _context.Houses
                    .Include(h => h.Owner)
                    .Include(h => h.Tenant)
                    .ToListAsync();
                return View("MyHouses", allHouses);
            }

            var houses = await _context.Houses
                .Where(h => h.OwnerId == owner!.OwnerId)
                .Include(h => h.Tenant)
                .ToListAsync();

            return View(houses);
        }

        // GET: Houses/Create
        [Authorize(Roles = "Owner,Administrator")]
        public async Task<IActionResult> Create()
        {
            if (User.IsInRole("Administrator"))
            {
                var owners = await _context.Owners
                    .Select(o => new
                    {
                        o.OwnerId,
                        FullName = o.First_Name + " " + o.Last_Name + " (" + o.Email + ")"
                    })
                    .ToListAsync();

                ViewBag.OwnerList = new SelectList(owners, "OwnerId", "FullName");
            }

            var cities = await _context.Cities.ToListAsync();
            ViewBag.CityList = new SelectList(cities, "CityId", "Name");

            return View();
        }


        // POST: Houses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Administrator")]
        public async Task<IActionResult> Create(House house, int? selectedOwnerId)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var isAdmin = User.IsInRole("Administrator");

                if (isAdmin)
                {
                    if (selectedOwnerId == null)
                    {
                        ModelState.AddModelError("", "Моля, избери собственик!");
                        return View(house);
                    }
                    house.OwnerId = selectedOwnerId.Value;
                }
                else
                {
                    var owner = await _context.Owners
                        .FirstOrDefaultAsync(o => o.ApplicationUserId == userId);

                    if (owner == null)
                        return Content("Owner not found");

                    house.OwnerId = owner.OwnerId;
                }

                house.Available = true;

                _context.Houses.Add(house);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.CityList = new SelectList(await _context.Cities.ToListAsync(), "CityId", "Name", house.CityId);

            return View(house);
        }

        // GET: Houses/Edit/5
        [Authorize(Roles = "Owner,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var house = await _context.Houses
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (house == null)
                return NotFound();

            if (User.IsInRole("Owner"))
            {
                var userId = _userManager.GetUserId(User);

                var owner = await _context.Owners
                    .FirstOrDefaultAsync(o => o.ApplicationUserId == userId);

                if (owner == null || house.OwnerId != owner.OwnerId)
                    return Forbid();
            }

            return View(house);
        }


        // POST: Houses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Administrator")]
        public async Task<IActionResult> Edit(int id, House house, int? newOwnerId)
        {
            if (id != house.HouseId)
                return NotFound();

            var existingHouse = await _context.Houses
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (existingHouse == null)
                return NotFound();

            if (User.IsInRole("Owner"))
            {
                var userId = _userManager.GetUserId(User);
                var owner = await _context.Owners
                    .FirstOrDefaultAsync(o => o.ApplicationUserId == userId);

                if (owner == null || existingHouse.OwnerId != owner.OwnerId)
                    return Forbid();
            }

            if (ModelState.IsValid)
            {
                existingHouse.Address = house.Address;
                existingHouse.Description = house.Description;
                existingHouse.Price_Per_Month = house.Price_Per_Month;
                existingHouse.Available = house.Available;

                if (User.IsInRole("Administrator") && newOwnerId.HasValue)
                {
                    existingHouse.OwnerId = newOwnerId.Value;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            if (User.IsInRole("Administrator") && newOwnerId.HasValue)
            {
                existingHouse.OwnerId = newOwnerId.Value;
            }

            if (User.IsInRole("Administrator") && house.CityId.HasValue)
            {
                existingHouse.CityId = house.CityId;
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
        [Authorize(Roles = "Owner,Administrator")]
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
