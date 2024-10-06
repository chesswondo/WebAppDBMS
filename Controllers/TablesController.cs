using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppDBMS.Models;

namespace WebAppDBMS.Controllers
{
    public class TablesController : Controller
    {
        private readonly DBContext _context;

        public TablesController(DBContext context)
        {
            _context = context;
        }

        // GET: Tables
        public async Task<IActionResult> Index(int? databaseId)
        {
            ViewBag.DatabaseId = databaseId;
            var databaseContext = _context.Tables.Where(t => t.DatabaseId == databaseId).Include(t => t.Database);
            return View(await databaseContext.ToListAsync());
        }

        // GET: Tables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .Include(t => t.Database)
                .Include(t => t.Columns)
                .Include(t => t.Rows)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index", "Rows", new { tableId = table.Id });
        }

        // GET: Tables/Create
        public IActionResult Create(int? databaseId)
        {
            if (databaseId == null)
            {
                return NotFound();
            }

            var table = new Table
            {
                DatabaseId = databaseId.Value,
                Database = _context.Databases.Where(db => db.Id == databaseId.Value).ToList()[0]
            };

            return View(table);
        }

        // POST: Tables/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Table table)
        {
            ModelState.Remove("Database");
            if (ModelState.IsValid)
            {
                // Fetch the database using DatabaseId and assign it to the Table's Database property
                table.Database = await _context.Databases.FindAsync(table.DatabaseId);

                if (table.Database == null)
                {
                    // Handle case where DatabaseId is invalid or doesn't exist
                    ModelState.AddModelError("", "Invalid database selected.");
                    return View(table);
                }

                _context.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Tables", new { databaseId = table.DatabaseId });
            }
            return View(table);
        }

        // GET: Tables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .Include(t => t.Database)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Load all related entities into memory first to avoid the DataReader issue
            var columns = await _context.Columns.Where(t => t.TableId == id).ToListAsync();
            var rows = await _context.Rows.Where(t => t.TableId == id).ToListAsync();

            // Retrieve and remove all cells for the rows
            foreach (var row in rows)
            {
                var cells = await _context.Cells.Where(t => t.RowId == row.Id).ToListAsync();
                _context.Cells.RemoveRange(cells);
            }

            // Remove the rows and columns
            _context.Rows.RemoveRange(rows);
            _context.Columns.RemoveRange(columns);

            // Find and remove the table itself
            var table = await _context.Tables.FindAsync(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
            }

            // Save all changes
            await _context.SaveChangesAsync();

            // Redirect to the index page for the tables
            return RedirectToAction("Index", "Tables", new { databaseId = table?.DatabaseId });
        }

        private bool TableExists(int id)
        {
            return _context.Tables.Any(e => e.Id == id);
        }

        // GET: Projection
        public IActionResult Proj(int? databaseId)
        {
            var db = _context.Databases.Include(t => t.Tables).First(t => t.Id == databaseId);
            // If no tables in database, return to previous state
            if (db.Tables.Count < 1)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            ViewBag.DatabaseId = databaseId;
            ViewBag.Tables = new SelectList(_context.Tables.Where(t => t.DatabaseId == databaseId), "Id", "Name");
            var firstTable = _context.Tables.Where(t => t.DatabaseId == databaseId).First();
            return View();
        }

        // Get columns by table id
        public IActionResult GetColumns(int tableId)
        {
            var columns = _context.Columns
                                  .Where(c => c.TableId == tableId)
                                  .Select(c => new { c.Id, c.Name })
                                  .ToList();
            return Json(columns);
        }

        // POST: Projection
        [HttpPost]
        public IActionResult TableProj(List<int> selectedColumns)
        {
            // Check if no columns are selected
            if (selectedColumns == null || !selectedColumns.Any())
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }

            int selectedTableId = _context.Tables
                                          .FirstOrDefault(t => t.Columns
                                          .Any(c => c.Id == selectedColumns[0])).Id;

            return RedirectToAction("ViewSelectedColumns", new { tableId = selectedTableId, selectedColumns });
        }


        public async Task<IActionResult> ViewSelectedColumns(int tableId, List<int> selectedColumns)
        {
            if (selectedColumns == null || !selectedColumns.Any())
            {
                return BadRequest("No columns selected.");
            }

            // Fetch the table with its rows and columns
            var table = await _context.Tables
                .Include(t => t.Rows)
                    .ThenInclude(r => r.Cells)
                .Include(t => t.Columns)
                .ThenInclude(c => c.Cells)
                .FirstOrDefaultAsync(t => t.Id == tableId);

            if (table == null)
            {
                return NotFound();
            }

            // Filter the columns based on the selected column IDs
            var filteredColumns = table.Columns.Where(c => selectedColumns.Contains(c.Id)).ToList();

            // Create a view model that contains the table, filtered columns, and rows
            var viewModel = new TableViewModel
            {
                Table = table,
                FilteredColumns = filteredColumns,
                Rows = table.Rows
            };

            return View(viewModel);
        }
    }
}
