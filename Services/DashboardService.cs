using FerticaStockManagement.Data;
using FerticaStockManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardAsync()
        {
            var model = new DashboardViewModel();

            // =========================
            // GENERAL STATISTICS
            // =========================

            model.TotalUsers = await _context.Users.CountAsync();

            model.TotalProducts = await _context.Products
                .CountAsync(p => p.IsActive);

            model.TotalRawMaterials = await _context.RawMaterials
                .CountAsync(r => r.IsActive);

            model.TotalSuppliers = await _context.Suppliers
                .CountAsync(s => s.IsActive);

            model.TotalProductionOrders =
                await _context.ProductionOrders.CountAsync();

            model.TotalStockEntries =
                await _context.StockEntries.CountAsync();

            model.TotalStockOutputs =
                await _context.StockOutputs.CountAsync();


            // =========================
            // STOCK
            // =========================

            model.TotalProductStock = await _context.Products
                .Where(p => p.IsActive)
                .SumAsync(p => p.CurrentStock);

            model.TotalRawMaterialStock = await _context.RawMaterials
                .Where(r => r.IsActive)
                .SumAsync(r => r.CurrentStock);


            // =========================
            // LOW STOCK
            // =========================

            model.LowStockProducts = await _context.Products
                .CountAsync(p =>
                    p.IsActive &&
                    p.CurrentStock <= p.MinimumStock);

            model.LowStockRawMaterials = await _context.RawMaterials
                .CountAsync(r =>
                    r.IsActive &&
                    r.CurrentStock <= r.MinimumStock);


            // =========================
            // PRODUCTION STATUS
            // =========================

            model.PendingProductionOrders =
                await _context.ProductionOrders
                    .CountAsync(p =>
                        p.Status == "Planifiée" ||
                        p.Status == "En attente");

            model.CompletedProductionOrders =
                await _context.ProductionOrders
                    .CountAsync(p =>
                        p.Status == "Terminée");


            // =========================
            // RECENT PRODUCTION
            // =========================

            model.RecentProductionOrders =
                await _context.ProductionOrders
                    .Include(p => p.Product)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new ProductionViewModel
                    {
                        Id = p.Id,
                        OrderNumber = p.OrderNumber,
                        ProductId = p.ProductId,
                        ProductName = p.Product != null
                            ? p.Product.Name
                            : string.Empty,
                        Status = p.Status,
                        PlannedQuantity = p.PlannedQuantity,
                        ProducedQuantity = p.ProducedQuantity,
                        PlannedDate = p.PlannedDate,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        Notes = p.Notes
                    })
                    .ToListAsync();


            // =========================
            // RECENT STOCK MOVEMENTS
            // =========================

            model.RecentStockMovements =
                await _context.StockMovements
                    .Include(m => m.RawMaterial)
                    .Include(m => m.Product)
                    .OrderByDescending(m => m.MovementDate)
                    .Take(10)
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
                                : string.Empty,

                        Quantity = m.Quantity,
                        StockBefore = m.StockBefore,
                        StockAfter = m.StockAfter,
                        MovementDate = m.MovementDate,
                        Description = m.Description,
                        CreatedBy = m.CreatedBy
                    })
                    .ToListAsync();

            return model;
        }
    }
}