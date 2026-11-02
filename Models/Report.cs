using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.Models
{
    public class Report
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        public string? GeneratedBy { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public string? FilePath { get; set; }

        // =============================
        // STATISTIQUES DU RAPPORT
        // =============================

        public decimal TotalStock { get; set; }

        public decimal TotalProduction { get; set; }

        public int TotalStockEntries { get; set; }

        public int TotalStockOutputs { get; set; }

        public int TotalSuppliers { get; set; }

        public int TotalProducts { get; set; }

        public int TotalRawMaterials { get; set; }

        public int TotalProductionOrders { get; set; }
    }
}