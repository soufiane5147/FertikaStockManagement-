using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class ProductionOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductionOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductionOrders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.ProductionOrders
                .Include(p => p.Product)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // GET: ProductionOrders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.ProductionOrders
                .Include(p => p.Product)
                .Include(p => p.ProductionDetails)
                    .ThenInclude(d => d.RawMaterial)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: Start
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int id)
        {
            var order = await _context.ProductionOrders
                .FindAsync(id);

            if (order == null)
                return NotFound();

            order.Status = "En cours";
            order.StartDate ??= DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var order = await _context.ProductionOrders
                .FindAsync(id);

            if (order == null)
                return NotFound();

            order.Status = "Terminée";
            order.EndDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.ProductionOrders
                .FindAsync(id);

            if (order == null)
                return NotFound();

            order.Status = "Annulée";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}