using TransportSchadules.Data;
using TransportSchadules.ViewModels;
using TransportSchadules.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Route = TransportSchadules.Models.Route;
using Microsoft.AspNetCore.Authorization;

namespace TransportSchadules.Controllers
{
    public class RoutesController : Controller
    {
        private readonly TransportDbContext _context;

        // Конструктор с внедрением зависимости
        public RoutesController(TransportDbContext context)
        {
            _context = context;
        }

        // 1. Вывод списка маршрутов (с фильтрацией)
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10; // Количество записей на странице

            ViewData["CurrentFilter"] = searchString;

            IQueryable<Route> routes = _context.Routes;

            if (!string.IsNullOrEmpty(searchString))
            {
                routes = routes.Where(r => r.Name.Contains(searchString));
            }

            int count = await routes.CountAsync();
            var items = await routes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new RoutesIndexViewModel
            {
                Routes = items,
                PageViewModel = new PageViewModel(count, page, pageSize)
            };

            return View(viewModel);
        }


        // 2. Детали маршрута
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var route = await _context.Routes
                .FirstOrDefaultAsync(m => m.RouteId == id);
            if (route == null) return NotFound();

            return View(route);
        }

        // 3. Добавление нового маршрута
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,TransportType,PlannedTravelTime,Distance,IsExpress")] Route route)
        {
            if (ModelState.IsValid)
            {
                _context.Add(route);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(route);
        }

        // 4. Редактирование маршрута
        // GET: Routes/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes.FindAsync(id);
            if (route == null)
            {
                return NotFound();
            }
            return View(route);
        }

        // POST: Routes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteId,Name,TransportType,PlannedTravelTime,Distance,IsExpress")] Route route)
        {
            if (id != route.RouteId)
            {
                return NotFound();
            }

            // Логирование для проверки значения IsExpress
            Console.WriteLine($"IsExpress: {route.IsExpress}");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(route);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteExists(route.RouteId))
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
            return View(route);
        }

        private bool RouteExists(int id)
        {
            return _context.Routes.Any(e => e.RouteId == id);
        }


        // 5. Удаление маршрута
        // GET: Routes/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes
                .FirstOrDefaultAsync(m => m.RouteId == id);
            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        // POST: Routes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
            {
                return NotFound();
            }

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
