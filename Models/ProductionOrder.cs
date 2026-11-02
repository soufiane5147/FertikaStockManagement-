using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.Models
{
    public class ProductionOrder
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Numéro de commande")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Range(0.01, double.MaxValue)]
        [Display(Name = "Quantité planifiée")]
        public decimal PlannedQuantity { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Quantité produite")]
        public decimal ProducedQuantity { get; set; } = 0;

        [Required]
        [StringLength(50)]
        [Display(Name = "Statut")]
        public string Status { get; set; } = "En attente";

        [Display(Name = "Date prévue")]
        public DateTime PlannedDate { get; set; } = DateTime.Now;

        [Display(Name = "Date de début")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Date de fin")]
        public DateTime? EndDate { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<ProductionDetail> ProductionDetails { get; set; }
            = new List<ProductionDetail>();
    }
}