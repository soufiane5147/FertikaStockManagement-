using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class StockEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // INDEX
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entries = await _context.StockEntries
                .Include(e => e.RawMaterial)
                .Include(e => e.Supplier)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();

            return View(entries);
        }

        // =========================================================
        // DETAILS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var entry = await _context.StockEntries
                .Include(e => e.RawMaterial)
                .Include(e => e.Supplier)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null)
                return NotFound();

            return View(entry);
        }

        // =========================================================
        // CREATE - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadListsAsync();

            var model = new StockEntryViewModel
            {
                EntryDate = DateTime.Now
            };

            return View(model);
        }

        // =========================================================
        // CREATE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockEntryViewModel model)
        {
            // Vérifier que la matière première existe
            var rawMaterial = await _context.RawMaterials
                .FirstOrDefaultAsync(r => r.Id == model.RawMaterialId);

            if (rawMaterial == null)
            {
                ModelState.AddModelError(
                    nameof(model.RawMaterialId),
                    "La matière première sélectionnée n'existe pas."
                );
            }

            // Vérifier la quantité
            if (model.Quantity <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.Quantity),
                    "La quantité doit être supérieure à zéro."
                );
            }

            // Vérifier le prix
            if (model.UnitPrice < 0)
            {
                ModelState.AddModelError(
                    nameof(model.UnitPrice),
                    "Le prix unitaire ne peut pas être négatif."
                );
            }

            if (!ModelState.IsValid)
            {
                await LoadListsAsync(model);
                return View(model);
            }

            // =====================================================
            // STOCK AVANT
            // =====================================================

            decimal stockBefore = rawMaterial!.CurrentStock;

            // =====================================================
            // NOUVEAU STOCK
            // =====================================================

            decimal stockAfter =
                stockBefore + model.Quantity;

            // =====================================================
            // CREATION DE L'ENTREE
            // =====================================================

            var entry = new StockEntry
            {
                Reference = model.Reference,
                RawMaterialId = model.RawMaterialId,
                SupplierId = model.SupplierId,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                EntryDate = model.EntryDate,
                Notes = model.Notes,
                CreatedBy = User.Identity?.Name
            };

            _context.StockEntries.Add(entry);

            // =====================================================
            // MISE A JOUR DU STOCK
            // =====================================================

            rawMaterial.CurrentStock = stockAfter;

            _context.RawMaterials.Update(rawMaterial);

            // =====================================================
            // CREATION DU MOUVEMENT
            // =====================================================

            var movement = new StockMovement
            {
                Reference = model.Reference,
                MovementType = "Entrée",
                RawMaterialId = rawMaterial.Id,
                ProductId = null,
                Quantity = model.Quantity,
                StockBefore = stockBefore,
                StockAfter = stockAfter,
                MovementDate = model.EntryDate,
                Description =
                    $"Entrée de {model.Quantity:N2} {rawMaterial.Unit} de matière première.",
                CreatedBy = User.Identity?.Name
            };

            _context.StockMovements.Add(movement);

            // =====================================================
            // SAUVEGARDE
            // =====================================================

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "L'entrée de stock a été enregistrée et le stock a été mis à jour.";

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // CHARGEMENT DES LISTES
        // =========================================================

        private async Task LoadListsAsync(
            StockEntryViewModel? model = null)
        {
            var rawMaterials = await _context.RawMaterials
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();

            var suppliers = await _context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.RawMaterials = new SelectList(
                rawMaterials,
                "Id",
                "Name",
                model?.RawMaterialId
            );

            ViewBag.Suppliers = new SelectList(
                suppliers,
                "Id",
                "Name",
                model?.SupplierId
            );
        }
    }
}