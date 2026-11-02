using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class StockOutputsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockOutputsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StockOutputs
        public async Task<IActionResult> Index()
        {
            var outputs = await _context.StockOutputs
                .Include(o => o.Product)
                .OrderByDescending(o => o.OutputDate)
                .ToListAsync();

            return View(outputs);
        }

        // GET: StockOutputs/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(new StockOutputViewModel
            {
                OutputDate = DateTime.Now
            });
        }

        // POST: StockOutputs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            StockOutputViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadProducts();
                return View(model);
            }

            var referenceExists = await _context.StockOutputs
                .AnyAsync(o => o.Reference == model.Reference);

            if (referenceExists)
            {
                ModelState.AddModelError(
                    nameof(model.Reference),
                    "Cette référence existe déjà."
                );

                await LoadProducts();
                return View(model);
            }

            var product = await _context.Products
                .FindAsync(model.ProductId);

            if (product == null)
                return NotFound();

            if (model.Quantity <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.Quantity),
                    "La quantité doit être supérieure à zéro."
                );

                await LoadProducts();
                return View(model);
            }

            if (model.Quantity > product.CurrentStock)
            {
                ModelState.AddModelError(
                    nameof(model.Quantity),
                    "Stock insuffisant."
                );

                await LoadProducts();
                return View(model);
            }

            var stockBefore = product.CurrentStock;

            product.CurrentStock -= model.Quantity;

            var output = new StockOutput
            {
                Reference = model.Reference,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                OutputDate = model.OutputDate,
                Reason = model.Reason,
                CreatedBy = User.Identity?.Name
            };

            _context.StockOutputs.Add(output);

            var movement = new StockMovement
            {
                Reference = model.Reference,
                MovementType = "Sortie",
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                StockBefore = stockBefore,
                StockAfter = product.CurrentStock,
                MovementDate = model.OutputDate,
                Description = model.Reason,
                CreatedBy = User.Identity?.Name
            };

            _context.StockMovements.Add(movement);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: StockOutputs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var output = await _context.StockOutputs
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (output == null)
                return NotFound();

            return View(output);
        }

        private async Task LoadProducts()
        {
            ViewBag.Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}