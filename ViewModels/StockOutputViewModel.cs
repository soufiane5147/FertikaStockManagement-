using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.ViewModels
{
    public class StockOutputViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Référence")]
        public string Reference { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Produit")]
        public int ProductId { get; set; }

        [Display(Name = "Produit")]
        public string? ProductName { get; set; }

        [Range(0.01, double.MaxValue)]
        [Display(Name = "Quantité")]
        public decimal Quantity { get; set; }

        [Display(Name = "Date de sortie")]
        public DateTime OutputDate { get; set; } = DateTime.Now;

        [StringLength(1000)]
        [Display(Name = "Motif")]
        public string? Reason { get; set; }

        public string? CreatedBy { get; set; }
    }
}