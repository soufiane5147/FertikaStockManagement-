using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class ProductionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Production
        public async Task<IActionResult> Index()
        {
            var productions = await _context.ProductionOrders
                .Include(p => p.Product)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(productions);
        }

        // GET: Production/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadData();

            var model = new ProductionViewModel();

            return View(model);
        }

        // POST: Production/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ProductionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return View(model);
            }

            var orderExists = await _context.ProductionOrders
                .AnyAsync(p => p.OrderNumber == model.OrderNumber);

            if (orderExists)
            {
                ModelState.AddModelError(
                    nameof(model.OrderNumber),
                    "Ce numéro d'ordre existe déjà."
                );

                await LoadData();
                return View(model);
            }

            var order = new ProductionOrder
            {
                OrderNumber = model.OrderNumber,
                ProductId = model.ProductId,
                Status = model.Status,
                PlannedQuantity = model.PlannedQuantity,
                ProducedQuantity = model.ProducedQuantity,
                PlannedDate = model.PlannedDate,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Notes = model.Notes,
                CreatedAt = DateTime.Now
            };

            _context.ProductionOrders.Add(order);

            await _context.SaveChangesAsync();

            if (model.Materials != null)
            {
                foreach (var material in model.Materials)
                {
                    if (material.RawMaterialId <= 0 ||
                        material.QuantityUsed <= 0)
                    {
                        continue;
                    }

                    var detail = new ProductionDetail
                    {
                        ProductionOrderId = order.Id,
                        RawMaterialId = material.RawMaterialId,
                        QuantityUsed = material.QuantityUsed,
                        Unit = material.Unit,
                        CreatedAt = DateTime.Now
                    };

                    _context.ProductionDetails.Add(detail);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(
                "Details",
                "ProductionOrders",
                new { id = order.Id }
            );
        }

        private async Task LoadData()
        {
            ViewBag.Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            ViewBag.RawMaterials = await _context.RawMaterials
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}