using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.ViewModels
{
    public class RawMaterialViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le code est obligatoire.")]
        [StringLength(100)]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(150)]
        [Display(Name = "Nom")]
        public string Name { get; set; } = string.Empty;

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
        [Display(Name = "Prix unitaire")]
        public decimal UnitPrice { get; set; }

        public int? SupplierId { get; set; }

        [Display(Name = "Fournisseur")]
        public string? SupplierName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}