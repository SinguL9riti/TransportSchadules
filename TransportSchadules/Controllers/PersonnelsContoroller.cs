using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TransportSchadules.Data;
using TransportSchadules.Models;

namespace TransportSchadules.Controllers
{
    public class PersonnelsController : Controller
    {
        private readonly TransportDbContext _context;

        public PersonnelsController(TransportDbContext context)
        {
            _context = context;
        }

        // GET: Personnels
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Personnel> personnels = _context.Personnels;

            if (!string.IsNullOrEmpty(searchString))
            {
                personnels = personnels.Where(p => p.EmployeeList.Contains(searchString));
            }

            return View(await personnels.ToListAsync());
        }

        // GET: Personnels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personnel = await _context.Personnels
                .FirstOrDefaultAsync(m => m.PersonnelId == id);
            if (personnel == null)
            {
                return NotFound();
            }

            return View(personnel);
        }

        // GET: Personnels/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Position, DateOfJoining")] Personnel personnel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personnel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personnel);
        }

        // GET: Personnels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personnel = await _context.Personnels.FindAsync(id);
            if (personnel == null)
            {
                return NotFound();
            }
            return View(personnel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonnelId, Name, Position, DateOfJoining")] Personnel personnel)
        {
            if (id != personnel.PersonnelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personnel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonnelExists(personnel.PersonnelId))
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
            return View(personnel);
        }

        // GET: Personnels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personnel = await _context.Personnels
                .FirstOrDefaultAsync(m => m.PersonnelId == id);
            if (personnel == null)
            {
                return NotFound();
            }

            return View(personnel);
        }

        // POST: Personnels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personnel = await _context.Personnels.FindAsync(id);
            _context.Personnels.Remove(personnel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonnelExists(int id)
        {
            return _context.Personnels.Any(e => e.PersonnelId == id);
        }
    }
}
