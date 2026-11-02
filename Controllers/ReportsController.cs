using FerticaStockManagement.Services;
using FerticaStockManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FerticaStockManagement.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ReportService _reportService;
        private readonly ReportFileService _reportFileService;

        public ReportsController(
            ReportService reportService,
            ReportFileService reportFileService)
        {
            _reportService = reportService;
            _reportFileService = reportFileService;
        }

        // =========================================================
        // INDEX
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reports = await _reportService.GetAllAsync();

            return View(reports);
        }

        // =========================================================
        // DETAILS
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var report = await _reportService.GetByIdAsync(id);

            if (report == null)
                return NotFound();

            return View(report);
        }

        // =========================================================
        // CREATE - GET
        // =========================================================

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ReportViewModel
            {
                StartDate = DateTime.Today.AddDays(-30),
                EndDate = DateTime.Today
            };

            return View(model);
        }

        // =========================================================
        // CREATE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ReportViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError(
                    nameof(model.EndDate),
                    "La date de fin doit être supérieure ou égale à la date de début."
                );

                return View(model);
            }

            var report = await _reportService.CreateAsync(
                model,
                User.Identity?.Name
            );

            return RedirectToAction(
                nameof(Details),
                new { id = report.Id }
            );
        }

        // =========================================================
        // EXPORT EXCEL
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ExportExcel(int id)
        {
            var report = await _reportService.GetByIdAsync(id);

            if (report == null)
                return NotFound();

            var file = await _reportFileService
                .GenerateExcelAsync(report);

            string fileName =
                $"Rapport_{report.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        // =========================================================
        // EXPORT PDF
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> ExportPdf(int id)
        {
            var report = await _reportService.GetByIdAsync(id);

            if (report == null)
                return NotFound();

            var file = await _reportFileService
                .GeneratePdfAsync(report);

            string fileName =
                $"Rapport_{report.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            return File(
                file,
                "application/pdf",
                fileName
            );
        }
    }
}