using FerticaStockManagement.Data;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class StockMovementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockMovementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StockMovements
        public async Task<IActionResult> Index()
        {
            var movements = await _context.StockMovements
                .Include(m => m.RawMaterial)
                .Include(m => m.Product)
                .OrderByDescending(m => m.MovementDate)
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
                .ToListAsync();

            return View(movements);
        }

        // GET: StockMovements/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var movement = await _context.StockMovements
                .Include(m => m.RawMaterial)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movement == null)
                return NotFound();

            var model = new StockMovementViewModel
            {
                Id = movement.Id,
                Reference = movement.Reference,
                MovementType = movement.MovementType,
                RawMaterialId = movement.RawMaterialId,
                ProductId = movement.ProductId,
                ItemName = movement.RawMaterial != null
                    ? movement.RawMaterial.Name
                    : movement.Product != null
                        ? movement.Product.Name
                        : "",
                Quantity = movement.Quantity,
                StockBefore = movement.StockBefore,
                StockAfter = movement.StockAfter,
                MovementDate = movement.MovementDate,
                Description = movement.Description,
                CreatedBy = movement.CreatedBy
            };

            return View(model);
        }
    }
}