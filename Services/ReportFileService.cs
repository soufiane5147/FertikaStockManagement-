using ClosedXML.Excel;
using FerticaStockManagement.Data;
using FerticaStockManagement.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FerticaStockManagement.Services
{
    public class ReportFileService
    {
        private readonly ApplicationDbContext _context;

        public ReportFileService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // EXCEL
        // =========================================================

        public async Task<byte[]> GenerateExcelAsync(Report report)
        {
            DateTime startDate = report.StartDate.Date;
            DateTime endDate = report.EndDate.Date.AddDays(1);

            var movements = await _context.StockMovements
                .Include(m => m.RawMaterial)
                .Include(m => m.Product)
                .Where(m =>
                    m.MovementDate >= startDate &&
                    m.MovementDate < endDate)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

            var productions = await _context.ProductionOrders
                .Include(p => p.Product)
                .Where(p =>
                    p.PlannedDate >= startDate &&
                    p.PlannedDate < endDate)
                .OrderByDescending(p => p.PlannedDate)
                .ToListAsync();

            var entries = await _context.StockEntries
                .Include(e => e.RawMaterial)
                .Include(e => e.Supplier)
                .Where(e =>
                    e.EntryDate >= startDate &&
                    e.EntryDate < endDate)
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();

            var outputs = await _context.StockOutputs
                .Include(o => o.Product)
                .Where(o =>
                    o.OutputDate >= startDate &&
                    o.OutputDate < endDate)
                .OrderByDescending(o => o.OutputDate)
                .ToListAsync();

            using var workbook = new XLWorkbook();

            // =====================================================
            // FEUILLE 1 : RESUME
            // =====================================================

            var summary = workbook.Worksheets.Add("Résumé");

            summary.Cell("A1").Value = report.Title;
            summary.Range("A1:B1").Merge();

            summary.Cell("A1").Style.Font.Bold = true;
            summary.Cell("A1").Style.Font.FontSize = 18;

            summary.Cell("A3").Value = "Type";
            summary.Cell("B3").Value = report.Type;

            summary.Cell("A4").Value = "Date début";
            summary.Cell("B4").Value = report.StartDate.ToString("dd/MM/yyyy");

            summary.Cell("A5").Value = "Date fin";
            summary.Cell("B5").Value = report.EndDate.ToString("dd/MM/yyyy");

            summary.Cell("A6").Value = "Généré le";
            summary.Cell("B6").Value = report.GeneratedAt.ToString("dd/MM/yyyy HH:mm");

            summary.Cell("A7").Value = "Généré par";
            summary.Cell("B7").Value = report.GeneratedBy ?? "";

            summary.Cell("A9").Value = "STATISTIQUES";
            summary.Range("A9:B9").Merge();

            summary.Cell("A10").Value = "Stock total";
            summary.Cell("B10").Value = report.TotalStock;

            summary.Cell("A11").Value = "Production totale";
            summary.Cell("B11").Value = report.TotalProduction;

            summary.Cell("A12").Value = "Entrées de stock";
            summary.Cell("B12").Value = report.TotalStockEntries;

            summary.Cell("A13").Value = "Sorties de stock";
            summary.Cell("B13").Value = report.TotalStockOutputs;

            summary.Cell("A14").Value = "Fournisseurs";
            summary.Cell("B14").Value = report.TotalSuppliers;

            summary.Cell("A15").Value = "Produits";
            summary.Cell("B15").Value = report.TotalProducts;

            summary.Cell("A16").Value = "Matières premières";
            summary.Cell("B16").Value = report.TotalRawMaterials;

            summary.Cell("A17").Value = "Ordres de production";
            summary.Cell("B17").Value = report.TotalProductionOrders;

            summary.Cell("A19").Value = "Description";
            summary.Cell("B19").Value = report.Description ?? "";

            summary.Columns().AdjustToContents();

            // =====================================================
            // FEUILLE 2 : MOUVEMENTS
            // =====================================================

            var movementSheet = workbook.Worksheets.Add("Mouvements");

            movementSheet.Cell("A1").Value = "Référence";
            movementSheet.Cell("B1").Value = "Type";
            movementSheet.Cell("C1").Value = "Article";
            movementSheet.Cell("D1").Value = "Quantité";
            movementSheet.Cell("E1").Value = "Stock avant";
            movementSheet.Cell("F1").Value = "Stock après";
            movementSheet.Cell("G1").Value = "Date";
            movementSheet.Cell("H1").Value = "Description";
            movementSheet.Cell("I1").Value = "Créé par";

            var movementHeader = movementSheet.Range("A1:I1");
            movementHeader.Style.Font.Bold = true;

            int movementRow = 2;

            foreach (var movement in movements)
            {
                string itemName = movement.RawMaterial != null
                    ? movement.RawMaterial.Name
                    : movement.Product != null
                        ? movement.Product.Name
                        : "";

                movementSheet.Cell(movementRow, 1).Value =
                    movement.Reference;

                movementSheet.Cell(movementRow, 2).Value =
                    movement.MovementType;

                movementSheet.Cell(movementRow, 3).Value =
                    itemName;

                movementSheet.Cell(movementRow, 4).Value =
                    movement.Quantity;

                movementSheet.Cell(movementRow, 5).Value =
                    movement.StockBefore;

                movementSheet.Cell(movementRow, 6).Value =
                    movement.StockAfter;

                movementSheet.Cell(movementRow, 7).Value =
                    movement.MovementDate.ToString("dd/MM/yyyy HH:mm");

                movementSheet.Cell(movementRow, 8).Value =
                    movement.Description ?? "";

                movementSheet.Cell(movementRow, 9).Value =
                    movement.CreatedBy ?? "";

                movementRow++;
            }

            movementSheet.Columns().AdjustToContents();

            // =====================================================
            // FEUILLE 3 : PRODUCTION
            // =====================================================

            var productionSheet = workbook.Worksheets.Add("Production");

            productionSheet.Cell("A1").Value = "N° commande";
            productionSheet.Cell("B1").Value = "Produit";
            productionSheet.Cell("C1").Value = "Quantité prévue";
            productionSheet.Cell("D1").Value = "Quantité produite";
            productionSheet.Cell("E1").Value = "Date prévue";
            productionSheet.Cell("F1").Value = "Date début";
            productionSheet.Cell("G1").Value = "Date fin";
            productionSheet.Cell("H1").Value = "Statut";

            var productionHeader = productionSheet.Range("A1:H1");
            productionHeader.Style.Font.Bold = true;

            int productionRow = 2;

            foreach (var production in productions)
            {
                productionSheet.Cell(productionRow, 1).Value =
                    production.OrderNumber;

                productionSheet.Cell(productionRow, 2).Value =
                    production.Product?.Name ?? "";

                productionSheet.Cell(productionRow, 3).Value =
                    production.PlannedQuantity;

                productionSheet.Cell(productionRow, 4).Value =
                    production.ProducedQuantity;

                productionSheet.Cell(productionRow, 5).Value =
                    production.PlannedDate.ToString("dd/MM/yyyy");

                productionSheet.Cell(productionRow, 6).Value =
                    production.StartDate?.ToString("dd/MM/yyyy") ?? "";

                productionSheet.Cell(productionRow, 7).Value =
                    production.EndDate?.ToString("dd/MM/yyyy") ?? "";

                productionSheet.Cell(productionRow, 8).Value =
                    production.Status;

                productionRow++;
            }

            productionSheet.Columns().AdjustToContents();

            // =====================================================
            // FEUILLE 4 : ENTREES
            // =====================================================

            var entrySheet = workbook.Worksheets.Add("Entrées");

            entrySheet.Cell("A1").Value = "Référence";
            entrySheet.Cell("B1").Value = "Matière première";
            entrySheet.Cell("C1").Value = "Fournisseur";
            entrySheet.Cell("D1").Value = "Quantité";
            entrySheet.Cell("E1").Value = "Prix unitaire";
            entrySheet.Cell("F1").Value = "Montant total";
            entrySheet.Cell("G1").Value = "Date";

            entrySheet.Range("A1:G1").Style.Font.Bold = true;

            int entryRow = 2;

            foreach (var entry in entries)
            {
                entrySheet.Cell(entryRow, 1).Value =
                    entry.Reference;

                entrySheet.Cell(entryRow, 2).Value =
                    entry.RawMaterial?.Name ?? "";

                entrySheet.Cell(entryRow, 3).Value =
                    entry.Supplier?.Name ?? "";

                entrySheet.Cell(entryRow, 4).Value =
                    entry.Quantity;

                entrySheet.Cell(entryRow, 5).Value =
                    entry.UnitPrice;

                entrySheet.Cell(entryRow, 6).Value =
                    entry.TotalAmount;

                entrySheet.Cell(entryRow, 7).Value =
                    entry.EntryDate.ToString("dd/MM/yyyy");

                entryRow++;
            }

            entrySheet.Columns().AdjustToContents();

            // =====================================================
            // FEUILLE 5 : SORTIES
            // =====================================================

            var outputSheet = workbook.Worksheets.Add("Sorties");

            outputSheet.Cell("A1").Value = "Référence";
            outputSheet.Cell("B1").Value = "Produit";
            outputSheet.Cell("C1").Value = "Quantité";
            outputSheet.Cell("D1").Value = "Date";
            outputSheet.Cell("E1").Value = "Motif";
            outputSheet.Cell("F1").Value = "Créé par";

            outputSheet.Range("A1:F1").Style.Font.Bold = true;

            int outputRow = 2;

            foreach (var output in outputs)
            {
                outputSheet.Cell(outputRow, 1).Value =
                    output.Reference;

                outputSheet.Cell(outputRow, 2).Value =
                    output.Product?.Name ?? "";

                outputSheet.Cell(outputRow, 3).Value =
                    output.Quantity;

                outputSheet.Cell(outputRow, 4).Value =
                    output.OutputDate.ToString("dd/MM/yyyy");

                outputSheet.Cell(outputRow, 5).Value =
                    output.Reason ?? "";

                outputSheet.Cell(outputRow, 6).Value =
                    output.CreatedBy ?? "";

                outputRow++;
            }

            outputSheet.Columns().AdjustToContents();

            // =====================================================
            // GENERATION DU FICHIER
            // =====================================================

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }


        // =========================================================
        // PDF
        // =========================================================

        public async Task<byte[]> GeneratePdfAsync(Report report)
        {
            DateTime startDate = report.StartDate.Date;
            DateTime endDate = report.EndDate.Date.AddDays(1);

            var movements = await _context.StockMovements
                .Include(m => m.RawMaterial)
                .Include(m => m.Product)
                .Where(m =>
                    m.MovementDate >= startDate &&
                    m.MovementDate < endDate)
                .OrderByDescending(m => m.MovementDate)
                .ToListAsync();

            var productions = await _context.ProductionOrders
                .Include(p => p.Product)
                .Where(p =>
                    p.PlannedDate >= startDate &&
                    p.PlannedDate < endDate)
                .OrderByDescending(p => p.PlannedDate)
                .ToListAsync();

            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.DefaultTextStyle(x =>
                        x.FontSize(9));

                    // =================================================
                    // HEADER
                    // =================================================

                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                .Text(report.Title)
                                .FontSize(20)
                                .Bold();

                            column.Item()
                                .Text($"Type : {report.Type}");

                            column.Item()
                                .Text(
                                    $"Période : {report.StartDate:dd/MM/yyyy} - {report.EndDate:dd/MM/yyyy}"
                                );

                            column.Item()
                                .PaddingBottom(10);
                        });

                    // =================================================
                    // CONTENU
                    // =================================================

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // -------------------------------
                            // Informations
                            // -------------------------------

                            column.Item()
                                .Text("Informations générales")
                                .FontSize(14)
                                .Bold();

                            column.Item()
                                .Text(
                                    $"Généré le : {report.GeneratedAt:dd/MM/yyyy HH:mm}"
                                );

                            column.Item()
                                .Text(
                                    $"Généré par : {report.GeneratedBy ?? "N/A"}"
                                );

                            if (!string.IsNullOrWhiteSpace(report.Description))
                            {
                                column.Item()
                                    .Text(
                                        $"Description : {report.Description}"
                                    );
                            }

                            // -------------------------------
                            // Statistiques
                            // -------------------------------

                            column.Item()
                                .PaddingTop(10)
                                .Text("Statistiques")
                                .FontSize(14)
                                .Bold();

                            column.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.ConstantColumn(100);
                                    });

                                    AddPdfRow(
                                        table,
                                        "Stock total",
                                        report.TotalStock.ToString("N2")
                                    );

                                    AddPdfRow(
                                        table,
                                        "Production totale",
                                        report.TotalProduction.ToString("N2")
                                    );

                                    AddPdfRow(
                                        table,
                                        "Entrées de stock",
                                        report.TotalStockEntries.ToString()
                                    );

                                    AddPdfRow(
                                        table,
                                        "Sorties de stock",
                                        report.TotalStockOutputs.ToString()
                                    );

                                    AddPdfRow(
                                        table,
                                        "Fournisseurs",
                                        report.TotalSuppliers.ToString()
                                    );

                                    AddPdfRow(
                                        table,
                                        "Produits",
                                        report.TotalProducts.ToString()
                                    );

                                    AddPdfRow(
                                        table,
                                        "Matières premières",
                                        report.TotalRawMaterials.ToString()
                                    );

                                    AddPdfRow(
                                        table,
                                        "Ordres de production",
                                        report.TotalProductionOrders.ToString()
                                    );
                                });

                            // -------------------------------
                            // Mouvements
                            // -------------------------------

                            column.Item()
                                .PaddingTop(10)
                                .Text("Mouvements de stock")
                                .FontSize(14)
                                .Bold();

                            if (movements.Count == 0)
                            {
                                column.Item()
                                    .Text("Aucun mouvement pendant cette période.");
                            }
                            else
                            {
                                column.Item()
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(1);
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("Référence");

                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("Type");

                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("Article");

                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("Qté");

                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("Date");
                                        });

                                        foreach (var movement in movements)
                                        {
                                            string itemName =
                                                movement.RawMaterial != null
                                                    ? movement.RawMaterial.Name
                                                    : movement.Product != null
                                                        ? movement.Product.Name
                                                        : "";

                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(movement.Reference);

                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(movement.MovementType);

                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(itemName);

                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(
                                                    movement.Quantity
                                                        .ToString("N2")
                                                );

                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(
                                                    movement.MovementDate
                                                        .ToString("dd/MM/yyyy")
                                                );
                                        }
                                    });
                            }

                            // -------------------------------
                            // Production
                            // -------------------------------

                            column.Item()
                                .PaddingTop(10)
                                .Text("Ordres de production")
                                .FontSize(14)
                                .Bold();

                            if (productions.Count == 0)
                            {
                                column.Item()
                                    .Text(
                                        "Aucun ordre de production pendant cette période."
                                    );
                            }
                            else
                            {
                                column.Item()
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(1);
                                            columns.RelativeColumn(1);
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("N°");

                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("Produit");

                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("Prévue");

                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("Produite");

                                            header.Cell().Element(PdfHeaderCell)
                                                .Text("Statut");
                                        });

                                        foreach (var production in productions)
                                        {
                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(production.OrderNumber);

                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(
                                                    production.Product?.Name
                                                    ?? ""
                                                );

                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(
                                                    production.PlannedQuantity
                                                        .ToString("N2")
                                                );

                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(
                                                    production.ProducedQuantity
                                                        .ToString("N2")
                                                );

                                            table.Cell()
                                                .Element(PdfBodyCell)
                                                .Text(production.Status);
                                        }
                                    });
                            }
                        });

                    // =================================================
                    // FOOTER
                    // =================================================

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });
            }).GeneratePdf();
        }


        // =========================================================
        // HELPERS PDF
        // =========================================================

        private static void AddPdfRow(
            TableDescriptor table,
            string label,
            string value)
        {
            table.Cell()
                .Element(PdfBodyCell)
                .Text(label);

            table.Cell()
                .Element(PdfBodyCell)
                .AlignRight()
                .Text(value);
        }

        private static IContainer PdfHeaderCell(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten2)
                .Border(1)
                .BorderColor(Colors.Grey.Medium)
                .Padding(5);
        }

        private static IContainer PdfBodyCell(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(5);
        }
    }
}
