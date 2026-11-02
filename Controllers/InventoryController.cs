using FerticaStockManagement.Data;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory
        public async Task<IActionResult> Index()
        {
            var model = new InventoryViewModel
            {
                RawMaterials = await _context.RawMaterials
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Name)
                    .Select(r => new RawMaterialInventoryViewModel
                    {
                        Id = r.Id,
                        Code = r.Code,
                        Name = r.Name,
                        Unit = r.Unit,
                        CurrentStock = r.CurrentStock,
                        MinimumStock = r.MinimumStock,
                        UnitPrice = r.UnitPrice
                    })
                    .ToListAsync(),

                Products = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .Select(p => new ProductInventoryViewModel
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        Unit = p.Unit,
                        CurrentStock = p.CurrentStock,
                        MinimumStock = p.MinimumStock,
                        SellingPrice = p.SellingPrice
                    })
                    .ToListAsync(),

                Movements = await _context.StockMovements
                    .Include(m => m.RawMaterial)
                    .Include(m => m.Product)
                    .OrderByDescending(m => m.MovementDate)
                    .Take(50)
                    .Select(m => new StockMovementViewModel
                    {
                        Id = m.Id,
                        Reference = m.Reference,
                        MovementType = m.MovementType,
                        RawMaterialId = m.RawMaterialId,
                        ProductId = m.ProductId,
                        ItemName = m.RawMaterial != null
                            ? m.RawMaterial.Name
                            : m.Product != null
                                ? m.Product.Name
                                : "",
                        Quantity = m.Quantity,
                        StockBefore = m.StockBefore,
                        StockAfter = m.StockAfter,
                        MovementDate = m.MovementDate,
                        Description = m.Description,
                        CreatedBy = m.CreatedBy
                    })
                    .ToListAsync()
            };

            model.RecentMovements = model.Movements
                .Take(10)
                .ToList();

            return View(model);
        }
    }
}