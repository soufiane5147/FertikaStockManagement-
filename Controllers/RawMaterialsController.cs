using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class RawMaterialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RawMaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RawMaterials
        public async Task<IActionResult> Index()
        {
            var materials = await _context.RawMaterials
                .Include(r => r.Supplier)
                .OrderBy(r => r.Name)
                .ToListAsync();

            return View(materials);
        }

        // GET: RawMaterials/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var material = await _context.RawMaterials
                .Include(r => r.Supplier)
                .Include(r => r.StockEntries)
                .Include(r => r.StockMovements)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (material == null)
                return NotFound();

            return View(material);
        }

        // GET: RawMaterials/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
        await LoadSuppliers();

        return View(new RawMaterialViewModel
        {
        IsActive = true
        });
        }

        // POST: RawMaterials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RawMaterialViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadSuppliers();
                return View(model);
            }

            var codeExists = await _context.RawMaterials
                .AnyAsync(r => r.Code == model.Code);

            if (codeExists)
            {
                ModelState.AddModelError(
                    nameof(model.Code),
                    "Ce code existe déjà."
                );

                await LoadSuppliers();
                return View(model);
            }

            var material = new RawMaterial
            {
                Code = model.Code,
                Name = model.Name,
                Unit = model.Unit,
                Type = model.Type,
                CurrentStock = model.CurrentStock,
                MinimumStock = model.MinimumStock,
                UnitPrice = model.UnitPrice,
                SupplierId = model.SupplierId,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.RawMaterials.Add(material);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: RawMaterials/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var material = await _context.RawMaterials
                .FindAsync(id);

            if (material == null)
                return NotFound();

            var model = new RawMaterialViewModel
            {
                Id = material.Id,
                Code = material.Code,
                Name = material.Name,
                Unit = material.Unit,
                Type = material.Type,
                CurrentStock = material.CurrentStock,
                MinimumStock = material.MinimumStock,
                UnitPrice = material.UnitPrice,
                SupplierId = material.SupplierId,
                IsActive = material.IsActive
            };

            await LoadSuppliers();

            return View(model);
        }

        // POST: RawMaterials/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            RawMaterialViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadSuppliers();
                return View(model);
            }

            var material = await _context.RawMaterials
                .FindAsync(model.Id);

            if (material == null)
                return NotFound();

            var codeExists = await _context.RawMaterials
                .AnyAsync(r =>
                    r.Code == model.Code &&
                    r.Id != model.Id);

            if (codeExists)
            {
                ModelState.AddModelError(
                    nameof(model.Code),
                    "Ce code existe déjà."
                );

                await LoadSuppliers();
                return View(model);
            }

            material.Code = model.Code;
            material.Name = model.Name;
            material.Unit = model.Unit;
            material.Type = model.Type;
            material.MinimumStock = model.MinimumStock;
            material.UnitPrice = model.UnitPrice;
            material.SupplierId = model.SupplierId;
            material.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: RawMaterials/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var material = await _context.RawMaterials
                .FindAsync(id);

            if (material == null)
                return NotFound();

            material.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSuppliers()
        {
            ViewBag.Suppliers = await _context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}