using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Product>> GetActiveAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductionOrders)
                .Include(p => p.StockOutputs)
                .Include(p => p.StockMovements)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> CodeExistsAsync(
            string code,
            int? excludeId = null)
        {
            return await _context.Products
                .AnyAsync(p =>
                    p.Code == code &&
                    (!excludeId.HasValue || p.Id != excludeId.Value));
        }

        public async Task<Product> CreateAsync(Product product)
        {
            product.CreatedAt = DateTime.Now;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var existing = await _context.Products
                .FindAsync(product.Id);

            if (existing == null)
                return false;

            existing.Code = product.Code;
            existing.Name = product.Name;
            existing.Unit = product.Unit;
            existing.Description = product.Description;
            existing.Type = product.Type;
            existing.MinimumStock = product.MinimumStock;
            existing.SellingPrice = product.SellingPrice;
            existing.CategoryId = product.CategoryId;
            existing.IsActive = product.IsActive;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var product = await _context.Products
                .FindAsync(id);

            if (product == null)
                return false;

            product.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HasLowStockAsync(int id)
        {
            var product = await _context.Products
                .FindAsync(id);

            if (product == null)
                return false;

            return product.CurrentStock <= product.MinimumStock;
        }
    }
}
