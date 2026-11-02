using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductionOrders)
                .Include(p => p.StockOutputs)
                .Include(p => p.StockMovements)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Products/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategories();

            return View(new ProductViewModel());
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(model);
            }

            var codeExists = await _context.Products
                .AnyAsync(p => p.Code == model.Code);

            if (codeExists)
            {
                ModelState.AddModelError(
                    nameof(model.Code),
                    "Ce code existe déjà."
                );

                await LoadCategories();
                return View(model);
            }

            var product = new Product
            {
                Code = model.Code,
                Name = model.Name,
                Unit = model.Unit,
                Description = model.Description,
                Type = model.Type,
                CurrentStock = model.CurrentStock,
                MinimumStock = model.MinimumStock,
                SellingPrice = model.SellingPrice,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                .FindAsync(id);

            if (product == null)
                return NotFound();

            var model = new ProductViewModel
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Unit = product.Unit,
                Description = product.Description,
                Type = product.Type,
                CurrentStock = product.CurrentStock,
                MinimumStock = product.MinimumStock,
                SellingPrice = product.SellingPrice,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive
            };

            await LoadCategories();

            return View(model);
        }

        // POST: Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View(model);
            }

            var product = await _context.Products
                .FindAsync(model.Id);

            if (product == null)
                return NotFound();

            var codeExists = await _context.Products
                .AnyAsync(p =>
                    p.Code == model.Code &&
                    p.Id != model.Id);

            if (codeExists)
            {
                ModelState.AddModelError(
                    nameof(model.Code),
                    "Ce code existe déjà."
                );

                await LoadCategories();
                return View(model);
            }

            product.Code = model.Code;
            product.Name = model.Name;
            product.Unit = model.Unit;
            product.Description = model.Description;
            product.Type = model.Type;
            product.MinimumStock = model.MinimumStock;
            product.SellingPrice = model.SellingPrice;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .FindAsync(id);

            if (product == null)
                return NotFound();

            product.IsActive = false;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories()
{
    var categories = await _context.Categories
        .Where(c => c.IsActive)
        .OrderBy(c => c.Name)
        .ToListAsync();

    ViewBag.Categories = new SelectList(
        categories,
        "Id",
        "Name"
    );
}
    }
}