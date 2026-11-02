using FerticaStockManagement.Models;
using FerticaStockManagement.Services;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class SuppliersController : Controller
    {
        private readonly SupplierService _supplierService;

        public SuppliersController(SupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // =========================================================
        // INDEX
        // =========================================================

        // GET: Suppliers
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllAsync();

            return View(suppliers);
        }

        // =========================================================
        // DETAILS
        // =========================================================

        // GET: Suppliers/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // =========================================================
        // CREATE
        // =========================================================

        // GET: Suppliers/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new SupplierViewModel
            {
                IsActive = true,
                Country = "Maroc"
            };

            return View(model);
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Vérifier si le nom existe déjà
            if (await _supplierService.NameExistsAsync(model.Name))
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Un fournisseur avec ce nom existe déjà."
                );

                return View(model);
            }

            // Conversion ViewModel → Model
            var supplier = new Supplier
            {
                Name = model.Name,
                ContactPerson = model.ContactPerson,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                IsActive = model.IsActive
            };

            await _supplierService.CreateAsync(supplier);

            TempData["SuccessMessage"] =
                "Le fournisseur a été ajouté avec succès.";

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // EDIT
        // =========================================================

        // GET: Suppliers/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);

            if (supplier == null)
                return NotFound();

            var model = new SupplierViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                City = supplier.City,
                Country = supplier.Country,
                IsActive = supplier.IsActive
            };

            return View(model);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            SupplierViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            // Vérifier le nom du fournisseur
            if (await _supplierService.NameExistsAsync(
                model.Name,
                model.Id))
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Un autre fournisseur avec ce nom existe déjà."
                );

                return View(model);
            }

            // Conversion ViewModel → Model
            var supplier = new Supplier
            {
                Id = model.Id,
                Name = model.Name,
                ContactPerson = model.ContactPerson,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                IsActive = model.IsActive
            };

            var success = await _supplierService.UpdateAsync(supplier);

            if (!success)
                return NotFound();

            TempData["SuccessMessage"] =
                "Le fournisseur a été modifié avec succès.";

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // DELETE / DEACTIVATE
        // =========================================================

        // GET: Suppliers/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);

            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _supplierService.DeactivateAsync(id);

            if (!success)
                return NotFound();

            TempData["SuccessMessage"] =
                "Le fournisseur a été désactivé avec succès.";

            return RedirectToAction(nameof(Index));
        }
    }
}