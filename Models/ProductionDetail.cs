using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.Models
{
    public class ProductionDetail
    {
        public int Id { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        public ProductionOrder? ProductionOrder { get; set; }

        [Required]
        public int RawMaterialId { get; set; }

        public RawMaterial? RawMaterial { get; set; }

        [Range(0.01, double.MaxValue)]
        [Display(Name = "Quantité utilisée")]
        public decimal QuantityUsed { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = "kg";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}