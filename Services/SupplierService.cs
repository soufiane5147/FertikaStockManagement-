using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Services
{
    public class SupplierService
    {
        private readonly ApplicationDbContext _context;

        public SupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers
                .Include(s => s.RawMaterials)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.RawMaterials)
                .Include(s => s.StockEntries)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Supplier?> GetActiveByIdAsync(int id)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            return await _context.Suppliers
                .AnyAsync(s =>
                    s.Name == name &&
                    (!excludeId.HasValue || s.Id != excludeId.Value));
        }

        public async Task<Supplier> CreateAsync(Supplier supplier)
        {
            supplier.CreatedAt = DateTime.Now;

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return supplier;
        }

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            var existing = await _context.Suppliers
                .FindAsync(supplier.Id);

            if (existing == null)
                return false;

            existing.Name = supplier.Name;
            existing.ContactPerson = supplier.ContactPerson;
            existing.Email = supplier.Email;
            existing.Phone = supplier.Phone;
            existing.Address = supplier.Address;
            existing.City = supplier.City;
            existing.Country = supplier.Country;
            existing.IsActive = supplier.IsActive;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var supplier = await _context.Suppliers
                .FindAsync(id);

            if (supplier == null)
                return false;

            supplier.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}