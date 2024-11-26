using TransportSchadules.Data;
using TransportSchadules.Models;
using TransportSchadules.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TransportSchadules.Controllers
{
    public class StopsController : Controller
    {
        private readonly TransportDbContext _context;
        public StopsController(TransportDbContext context)
        {
            _context = context;
        }

        // 1. Вывод списка остановок (с фильтрацией)
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10; // Количество записей на странице

            var stopsQuery = _context.Stops.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                stopsQuery = stopsQuery.Where(s => s.Name.Contains(searchString));
            }

            var count = await stopsQuery.CountAsync(); // Общее количество записей
            var items = await stopsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(); // Выбираем записи для текущей страницы

            var viewModel = new StopsViewModel
            {
                Stops = items,
                PageViewModel = new PageViewModel(count, page, pageSize)
            };

            return View(viewModel);
        }


        // 2. Детали остановки
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var stop = await _context.Stops.FirstOrDefaultAsync(m => m.StopId == id);
            if (stop == null) return NotFound();

            return View(stop);
        }

        // 3. Добавление новой остановки
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IsTerminal,HasDispatcher")] Stop stop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stop);
        }

        // 4. Редактирование остановки
        // GET: Stops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stop = await _context.Stops.FindAsync(id);
            if (stop == null)
            {
                return NotFound();
            }

            return View(stop);
        }


        // POST: Stops/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StopId,Name,IsTerminal,HasDispatcher")] Stop stop)
        {
            if (id != stop.StopId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(stop);
            }

            try
            {
                _context.Update(stop);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Stops.Any(e => e.StopId == stop.StopId))
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


        // 5. Удаление остановки
        // GET: Stops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stop = await _context.Stops
                .FirstOrDefaultAsync(m => m.StopId == id);
            if (stop == null)
            {
                return NotFound();
            }

            return View(stop);
        }

        // POST: Stops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stop = await _context.Stops.FindAsync(id);
            if (stop == null)
            {
                return NotFound();
            }

            _context.Stops.Remove(stop);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}