using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppDBMS.Models;
using Column = WebAppDBMS.Models.Column;

namespace WebAppDBMS.Controllers
{
    public class ColumnsController : Controller
    {
        private readonly DBContext _context;

        public ColumnsController(DBContext context)
        {
            _context = context;
        }

        // GET: Columns
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Columns.Include(c => c.Table);
            return View(await dBContext.ToListAsync());
        }

        // GET: Columns/Create
        public IActionResult Create(int? tableId)
        {
            if (tableId == null)
            {
                return NotFound();
            }

            var hasRows = _context.Rows.Any(r => r.TableId == tableId);

            if (hasRows)
            {
                ViewBag.ErrorMessage = "You cannot add new columns when your table is not empty.";
                ViewBag.TableId = tableId;
                return View("AddColumnError"); // We'll render an error view in this case
            }

            ViewBag.TableId = tableId;
            return View();
        }

        // POST: Columns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,TypeFullName,TableId")] Column column)
        {
            ModelState.Remove("Table");

            // Check again in case rows were added between GET and POST
            var hasRows = _context.Rows.Any(r => r.TableId == column.TableId);
            if (hasRows)
            {
                ViewBag.ErrorMessage = "You cannot add new columns when your table is not empty.";
                ViewBag.TableId = column.TableId;
                return View("AddColumnError");
            }

            if (ModelState.IsValid)
            {
                var rows = _context.Rows.Where(t => t.TableId == column.TableId);
                foreach (var row in rows)
                {
                    Cell cell = new Cell()
                    {
                        ColumnID = column.Id,
                        RowId = row.Id,
                        Value = null
                    };
                    _context.Add(cell);
                    column.Cells.Add(cell);
                }
                _context.Add(column);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Rows", new { tableId = column.TableId });
            }
            return View(column);
        }

        // GET: Columns/Delete/5
        public async Task<IActionResult> Delete(int? tableId)
        {
            if (tableId == null)
            {
                return NotFound();
            }
            var column = await _context.Columns
                .Include(c => c.Table).Where(m => m.TableId == tableId).ToListAsync();
            if (column == null)
            {
                return NotFound();
            }
            ViewBag.TableId = tableId;
            ViewData["Id"] = new SelectList(column, "Id", "Name");
            return View();
        }

        // POST: Columns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("Id, TableId")] Column column)
        {
            var col = await _context.Columns.FindAsync(column.Id);
            var cells = _context.Cells.Where(t => t.ColumnID == column.Id);
            _context.Cells.RemoveRange(cells);
            _context.Columns.Remove(col);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Rows", new { tableId = column.TableId });
        }

        private bool ColumnExists(int id)
        {
            return _context.Columns.Any(e => e.Id == id);
        }

        // --CHECK LATER--
        public IActionResult TypeValid(string? TypeFullName)
        {
            if (TypeFullName.Contains("Enum") || TypeFullName.Contains("EmailAddress"))
            {
                return Json(true);
            }

            else if (!TypeFullName.Contains("EmailAddress") && !TypeFullName.Contains("Enum"))
            {
                var type = Type.GetType(TypeFullName);
                if (type == null)
                    return Json(data: "Invalid type");
                else return Json(true);

            }
            else
            {
                return Json(data: "Invalid type");
            }
        }
    }
}
