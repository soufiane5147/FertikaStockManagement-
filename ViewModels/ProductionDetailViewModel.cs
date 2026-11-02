using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.ViewModels
{
    public class ProductionDetailViewModel
    {
        public int Id { get; set; }

        public int ProductionOrderId { get; set; }

        public int RawMaterialId { get; set; }

        [Display(Name = "Matière première")]
        public string? RawMaterialName { get; set; }

        [Range(0.01, double.MaxValue)]
        [Display(Name = "Quantité utilisée")]
        public decimal QuantityUsed { get; set; }

        [StringLength(50)]
        [Display(Name = "Unité")]
        public string Unit { get; set; } = "kg";
    }
}