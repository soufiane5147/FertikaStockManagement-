using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.ViewModels
{
    public class StockEntryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La référence est obligatoire.")]
        [Display(Name = "Référence")]
        public string Reference { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matière première est obligatoire.")]
        [Display(Name = "Matière première")]
        public int RawMaterialId { get; set; }

        [Display(Name = "Fournisseur")]
        public int? SupplierId { get; set; }

        [Required(ErrorMessage = "La quantité est obligatoire.")]
        [Range(
            0.001,
            double.MaxValue,
            ErrorMessage = "La quantité doit être supérieure à zéro."
        )]
        [Display(Name = "Quantité")]
        public decimal Quantity { get; set; }

        [Range(
            0,
            double.MaxValue,
            ErrorMessage = "Le prix ne peut pas être négatif."
        )]
        [Display(Name = "Prix unitaire")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Montant total")]
        public decimal TotalAmount =>
            Quantity * UnitPrice;

        [Required(ErrorMessage = "La date est obligatoire.")]
        [Display(Name = "Date d'entrée")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Créé par")]
        public string? CreatedBy { get; set; }
    }
}