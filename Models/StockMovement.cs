using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.Models
{
    public class StockMovement
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Reference { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [Display(Name = "Type de mouvement")]
        public string MovementType { get; set; } = string.Empty;

        public int? RawMaterialId { get; set; }

        public RawMaterial? RawMaterial { get; set; }

        public int? ProductId { get; set; }

        public Product? Product { get; set; }

        public decimal Quantity { get; set; }

        public decimal StockBefore { get; set; }

        public decimal StockAfter { get; set; }

        public DateTime MovementDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Description { get; set; }

        public string? CreatedBy { get; set; }
    }
}