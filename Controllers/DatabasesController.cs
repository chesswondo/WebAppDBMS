using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppDBMS.Models;

namespace WebAppDBMS.Controllers
{
    public class DatabasesController : Controller
    {
        private readonly DBContext _context;

        public DatabasesController(DBContext context)
        {
            _context = context;
        }

        // GET: Databases
        public async Task<IActionResult> Index()
        {
            return View(await _context.Databases.ToListAsync());
        }

        // GET: Databases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.DatabaseId = id;
            var database = await _context.Databases
                .FirstOrDefaultAsync(m => m.Id == id);
            if (database == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Tables", new { databaseId = database.Id });
        }

        // GET: Databases/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Databases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Database database)
        {
            if (ModelState.IsValid)
            {
                _context.Add(database);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(database);
        }

        // GET: Databases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var database = await _context.Databases
                .FirstOrDefaultAsync(m => m.Id == id);
            if (database == null)
            {
                return NotFound();
            }

            return View(database);
        }

        // POST: Databases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tables = _context.Tables.Where(t => t.DatabaseId == id);
            foreach (var table in tables)
            {
                var columns = _context.Columns.Where(t => t.TableId == table.Id);
                var rows = _context.Rows.Where(t => t.TableId == table.Id);
                foreach (var row in rows)
                {
                    var cells = _context.Cells.Where(t => t.RowId == row.Id);
                    _context.RemoveRange(cells);
                }
                _context.RemoveRange(rows);
                _context.RemoveRange(columns);
            }
            _context.RemoveRange(tables);
            var database = await _context.Databases.FindAsync(id);
            _context.Databases.Remove(database);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatabaseExists(int id)
        {
            return _context.Databases.Any(e => e.Id == id);
        }
    }
}
