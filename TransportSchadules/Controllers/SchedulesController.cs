using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransportSchadules.Data;
using TransportSchadules.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Route = TransportSchadules.Models.Route;
using TransportSchadules.ViewModels;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Index(string routeSearchString, string stopSearchString, int page = 1)
        {
            int pageSize = 10; // Количество записей на странице

            ViewData["CurrentRouteFilter"] = routeSearchString;
            ViewData["CurrentStopFilter"] = stopSearchString;

            IQueryable<Schedule> schedules = _context.Schedules
                .Include(s => s.Route)
                .Include(s => s.Stop);

            if (!string.IsNullOrEmpty(routeSearchString))
            {
                schedules = schedules.Where(s => s.Route.Name.Contains(routeSearchString));
            }

            if (!string.IsNullOrEmpty(stopSearchString))
            {
                schedules = schedules.Where(s => s.Stop.Name.Contains(stopSearchString));
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
                .Include(s => s.Route)  // Загружаем маршрут
                .Include(s => s.Stop)   // Загружаем остановку
                .FirstOrDefaultAsync(m => m.ScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }


        // GET: Schedules/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            ViewData["StopId"] = new SelectList(_context.Stops, "StopId", "Name");
            ViewData["RouteId"] = new SelectList(_context.Routes, "RouteId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteId,StopId,Weekday,ArrivalTime,Year")] ScheduleViewModel scheduleViewModel)
        {
            if (ModelState.IsValid)
            {
                var schedule = new Schedule
                {
                    RouteId = scheduleViewModel.RouteId,
                    StopId = scheduleViewModel.StopId,
                    Weekday = scheduleViewModel.Weekday,
                    ArrivalTime = scheduleViewModel.ArrivalTime,
                    Year = scheduleViewModel.Year
                };

                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["StopId"] = new SelectList(_context.Stops, "StopId", "Name", scheduleViewModel.StopId);
            ViewData["RouteId"] = new SelectList(_context.Routes, "RouteId", "Name", scheduleViewModel.RouteId);
            return View(scheduleViewModel);
        }

        // Редактирование расписания
        [Authorize(Roles = "admin")]
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

            ViewData["StopId"] = new SelectList(_context.Stops, "StopId", "Name", schedule.StopId);
            ViewData["RouteId"] = new SelectList(_context.Routes, "RouteId", "Name", schedule.RouteId); 
            return View(schedule);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,RouteId,StopId,Weekday,ArrivalTime,Year")] ScheduleIdViewModel scheduleIdViewModel)
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
                    schedule.StopId = scheduleIdViewModel.StopId;
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
            ViewData["StopId"] = new SelectList(_context.Stops, "StopId", "Name", schedule.StopId);
            ViewData["RouteId"] = new SelectList(_context.Routes, "RouteId", "Name", schedule.RouteId);
            return View(schedule);
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ScheduleId == id);
        }

        // GET: Schedules/Delete/5
        [Authorize(Roles = "admin")]
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
