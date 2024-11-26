using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSchadules.Data;
using TransportSchadules.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Route = TransportSchadules.Models.Route;
using TransportSchadules.ViewModels;

namespace TransportSchadules.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly TransportDbContext _context;

        public SchedulesController(TransportDbContext context)
        {
            _context = context;
        }

        // GET: Schedules
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10; // Количество записей на странице

            ViewData["CurrentFilter"] = searchString;

            IQueryable<Schedule> schedules = _context.Schedules.Include(s => s.Route);

            if (!string.IsNullOrEmpty(searchString))
            {
                schedules = schedules.Where(s => s.Route.Name.Contains(searchString));
            }

            int count = await schedules.CountAsync();
            var items = await schedules
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new SchedulesIndexViewModel
            {
                Schedules = items,
                PageViewModel = new PageViewModel(count, page, pageSize)
            };

            return View(viewModel);
        }



        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Route)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: Schedules/Create
        public IActionResult Create()
        {
            ViewData["RouteId"] = new SelectList(_context.Routes, "RouteId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteId,Weekday,ArrivalTime,Year")] ScheduleViewModel scheduleViewModel)
        {
            if (ModelState.IsValid)
            {
                //var route = _context.Routes.FirstOrDefault(i => i.RouteId == scheduleViewModel.RouteId);
                _context.Add(new Schedule
                {
                    RouteId = scheduleViewModel.RouteId,
                    Weekday = scheduleViewModel.Weekday,
                    ArrivalTime = scheduleViewModel.ArrivalTime,
                    Year = scheduleViewModel.Year,
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["RouteId"] = new SelectList(_context.Routes, "RouteId", "Name", scheduleViewModel.RouteId);
            return View(scheduleViewModel);
        }

        // Редактирование расписания
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            ViewData["RouteId"] = new SelectList(_context.Routes, "RouteId", "Name", schedule.RouteId);
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,RouteId,Weekday,ArrivalTime,Year")] ScheduleIdViewModel scheduleIdViewModel)
        {
            if (id != scheduleIdViewModel.ScheduleId)
            {
                return NotFound();
            }

            var schedule = _context.Schedules.Include(r => r.Route).FirstOrDefault(s => s.ScheduleId == scheduleIdViewModel.ScheduleId);


            if (ModelState.IsValid)
            {
                try
                {
                    schedule.ScheduleId = scheduleIdViewModel.ScheduleId;
                    schedule.RouteId = scheduleIdViewModel.RouteId;
                    schedule.Weekday = scheduleIdViewModel.Weekday;
                    schedule.ArrivalTime = scheduleIdViewModel.ArrivalTime;
                    schedule.Year = scheduleIdViewModel.Year;
                    schedule.Route = _context.Routes.FirstOrDefault(r => r.RouteId == scheduleIdViewModel.RouteId);
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ScheduleId))
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
            ViewData["RouteId"] = new SelectList(_context.Routes, "RouteId", "Name", schedule.RouteId);
            return View(schedule);
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ScheduleId == id);
        }

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Route)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
