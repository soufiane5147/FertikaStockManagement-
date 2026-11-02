using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using FerticaStockManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Services
{
    public class ReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Report>> GetAllAsync()
        {
            return await _context.Reports
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Report> CreateAsync(
            ReportViewModel model,
            string? generatedBy)
        {
            // -----------------------------
            // STOCK TOTAL
            // -----------------------------

            var rawMaterialStock = await _context.RawMaterials
                .Where(r => r.IsActive)
                .SumAsync(r => (decimal?)r.CurrentStock) ?? 0;

            var productStock = await _context.Products
                .Where(p => p.IsActive)
                .SumAsync(p => (decimal?)p.CurrentStock) ?? 0;

            var totalStock = rawMaterialStock + productStock;


            // -----------------------------
            // PRODUCTION TOTAL
            // -----------------------------

            var totalProduction = await _context.ProductionOrders
                .Where(p =>
                    p.PlannedDate.Date >= model.StartDate.Date &&
                    p.PlannedDate.Date <= model.EndDate.Date)
                .SumAsync(p => (decimal?)p.ProducedQuantity) ?? 0;


            // -----------------------------
            // STOCK ENTRIES
            // -----------------------------

            var totalStockEntries = await _context.StockEntries
                .CountAsync(e =>
                    e.EntryDate.Date >= model.StartDate.Date &&
                    e.EntryDate.Date <= model.EndDate.Date);


            // -----------------------------
            // STOCK OUTPUTS
            // -----------------------------

            var totalStockOutputs = await _context.StockOutputs
                .CountAsync(o =>
                    o.OutputDate.Date >= model.StartDate.Date &&
                    o.OutputDate.Date <= model.EndDate.Date);


            // -----------------------------
            // SUPPLIERS
            // -----------------------------

            var totalSuppliers = await _context.Suppliers
                .CountAsync(s => s.IsActive);


            // -----------------------------
            // PRODUCTS
            // -----------------------------

            var totalProducts = await _context.Products
                .CountAsync(p => p.IsActive);


            // -----------------------------
            // RAW MATERIALS
            // -----------------------------

            var totalRawMaterials = await _context.RawMaterials
                .CountAsync(r => r.IsActive);


            // -----------------------------
            // PRODUCTION ORDERS
            // -----------------------------

            var totalProductionOrders = await _context.ProductionOrders
                .CountAsync(p =>
                    p.PlannedDate.Date >= model.StartDate.Date &&
                    p.PlannedDate.Date <= model.EndDate.Date);


            // -----------------------------
            // CREATE REPORT
            // -----------------------------

            var report = new Report
{
    Title = model.Title,
    Type = model.Type,
    StartDate = model.StartDate,
    EndDate = model.EndDate,
    GeneratedAt = DateTime.Now,
    GeneratedBy = generatedBy,
    Description = model.Description,

    TotalStock = totalStock,
    TotalProduction = totalProduction,
    TotalStockEntries = totalStockEntries,
    TotalStockOutputs = totalStockOutputs,
    TotalSuppliers = totalSuppliers,
    TotalProducts = totalProducts,
    TotalRawMaterials = totalRawMaterials,
    TotalProductionOrders = totalProductionOrders
};

            _context.Reports.Add(report);

            await _context.SaveChangesAsync();

            return report;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.Reports
                .FindAsync(id);

            if (report == null)
                return false;

            _context.Reports.Remove(report);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}