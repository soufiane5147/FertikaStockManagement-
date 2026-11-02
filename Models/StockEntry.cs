using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.Models
{
    public class StockEntry
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Référence")]
        public string Reference { get; set; } = string.Empty;

        [Required]
        public int RawMaterialId { get; set; }

        public RawMaterial? RawMaterial { get; set; }

        public int? SupplierId { get; set; }

        public Supplier? Supplier { get; set; }

        [Range(0.01, double.MaxValue)]
        [Display(Name = "Quantité")]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Prix unitaire")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Montant total")]
        public decimal TotalAmount => Quantity * UnitPrice;

        [Display(Name = "Date d'entrée")]
        public DateTime EntryDate { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public string? CreatedBy { get; set; }
    }
}