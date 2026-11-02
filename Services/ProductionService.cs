using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using FerticaStockManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Services
{
    public class ProductionService
    {
        private readonly ApplicationDbContext _context;

        public ProductionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductionOrder>> GetAllAsync()
        {
            return await _context.ProductionOrders
                .Include(p => p.Product)
                .Include(p => p.ProductionDetails)
                    .ThenInclude(d => d.RawMaterial)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<ProductionOrder?> GetByIdAsync(int id)
        {
            return await _context.ProductionOrders
                .Include(p => p.Product)
                .Include(p => p.ProductionDetails)
                    .ThenInclude(d => d.RawMaterial)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> OrderNumberExistsAsync(
            string orderNumber,
            int? excludeId = null)
        {
            return await _context.ProductionOrders
                .AnyAsync(p =>
                    p.OrderNumber == orderNumber &&
                    (!excludeId.HasValue || p.Id != excludeId.Value));
        }

        public async Task<ProductionOrder> CreateAsync(
            ProductionViewModel model)
        {
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

            return order;
        }

        public async Task<bool> StartAsync(int id)
        {
            var order = await _context.ProductionOrders
                .FindAsync(id);

            if (order == null)
                return false;

            order.Status = "En cours";
            order.StartDate ??= DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CompleteAsync(int id)
        {
            var order = await _context.ProductionOrders
                .FindAsync(id);

            if (order == null)
                return false;

            order.Status = "Terminée";
            order.EndDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var order = await _context.ProductionOrders
                .FindAsync(id);

            if (order == null)
                return false;

            order.Status = "Annulée";

            await _context.SaveChangesAsync();

            return true;
        }
    }
}