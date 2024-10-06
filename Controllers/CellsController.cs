using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppDBMS.Models;

namespace WebAppDBMS.Controllers
{
    public class CellsController : Controller
    {
        private readonly DBContext _context;

        public CellsController(DBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Cells.Include(c => c.Column).Include(c => c.Row);
            return View(await databaseContext.ToListAsync());
        }
    }
}
