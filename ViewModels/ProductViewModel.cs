using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Nom")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(100)]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Unité")]
        public string Unit { get; set; } = "kg";

        [Range(0, double.MaxValue)]
        [Display(Name = "Stock actuel")]
        public decimal CurrentStock { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Stock minimum")]
        public decimal MinimumStock { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Prix de vente")]
        public decimal SellingPrice { get; set; }

        public int? CategoryId { get; set; }

        [Display(Name = "Catégorie")]
        public string? CategoryName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}