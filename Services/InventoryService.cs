using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using FerticaStockManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Services
{
    public class InventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryViewModel> GetInventoryAsync()
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

                Movements = await GetRecentMovementsAsync(50)
            };

            model.RecentMovements = model.Movements
                .Take(10)
                .ToList();

            return model;
        }

        public async Task<List<StockMovementViewModel>>
            GetRecentMovementsAsync(int count = 50)
        {
            return await _context.StockMovements
                .Include(m => m.RawMaterial)
                .Include(m => m.Product)
                .OrderByDescending(m => m.MovementDate)
                .Take(count)
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
        }

        public async Task<bool> AddRawMaterialStockAsync(
            int rawMaterialId,
            decimal quantity)
        {
            if (quantity <= 0)
                return false;

            var material = await _context.RawMaterials
                .FindAsync(rawMaterialId);

            if (material == null)
                return false;

            material.CurrentStock += quantity;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveProductStockAsync(
            int productId,
            decimal quantity)
        {
            if (quantity <= 0)
                return false;

            var product = await _context.Products
                .FindAsync(productId);

            if (product == null)
                return false;

            if (product.CurrentStock < quantity)
                return false;

            product.CurrentStock -= quantity;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}