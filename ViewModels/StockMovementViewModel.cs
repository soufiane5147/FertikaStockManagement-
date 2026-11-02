using System;
using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.ViewModels
{
    public class StockMovementViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Référence")]
        public string Reference { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Type de mouvement")]
        public string MovementType { get; set; } = string.Empty;

        public int? RawMaterialId { get; set; }

        public int? ProductId { get; set; }

        [Display(Name = "Article")]
        public string ItemName { get; set; } = string.Empty;

        [Range(0.001, double.MaxValue)]
        [Display(Name = "Quantité")]
        public decimal Quantity { get; set; }

        [Display(Name = "Stock avant")]
        public decimal StockBefore { get; set; }

        [Display(Name = "Stock après")]
        public decimal StockAfter { get; set; }

        [Display(Name = "Date")]
        public DateTime MovementDate { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Créé par")]
        public string? CreatedBy { get; set; }
    }
}