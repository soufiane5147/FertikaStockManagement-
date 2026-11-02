using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.Models
{
    public class StockOutput
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Référence")]
        public string Reference { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

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