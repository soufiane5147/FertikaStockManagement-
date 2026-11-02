using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Services
{
    public class RawMaterialService
    {
        private readonly ApplicationDbContext _context;

        public RawMaterialService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RawMaterial>> GetAllAsync()
        {
            return await _context.RawMaterials
                .Include(r => r.Supplier)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<RawMaterial>> GetActiveAsync()
        {
            return await _context.RawMaterials
                .Include(r => r.Supplier)
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<RawMaterial?> GetByIdAsync(int id)
        {
            return await _context.RawMaterials
                .Include(r => r.Supplier)
                .Include(r => r.StockEntries)
                .Include(r => r.StockMovements)
                .Include(r => r.ProductionDetails)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> CodeExistsAsync(
            string code,
            int? excludeId = null)
        {
            return await _context.RawMaterials
                .AnyAsync(r =>
                    r.Code == code &&
                    (!excludeId.HasValue || r.Id != excludeId.Value));
        }

        public async Task<RawMaterial> CreateAsync(
            RawMaterial material)
        {
            material.CreatedAt = DateTime.Now;

            _context.RawMaterials.Add(material);
            await _context.SaveChangesAsync();

            return material;
        }

        public async Task<bool> UpdateAsync(
            RawMaterial material)
        {
            var existing = await _context.RawMaterials
                .FindAsync(material.Id);

            if (existing == null)
                return false;

            existing.Code = material.Code;
            existing.Name = material.Name;
            existing.Unit = material.Unit;
            existing.Type = material.Type;
            existing.MinimumStock = material.MinimumStock;
            existing.UnitPrice = material.UnitPrice;
            existing.SupplierId = material.SupplierId;
            existing.IsActive = material.IsActive;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var material = await _context.RawMaterials
                .FindAsync(id);

            if (material == null)
                return false;

            material.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HasLowStockAsync(int id)
        {
            var material = await _context.RawMaterials
                .FindAsync(id);

            if (material == null)
                return false;

            return material.CurrentStock <= material.MinimumStock;
        }
    }
}