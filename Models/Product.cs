using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.Models
{
    public class Product
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
        public decimal CurrentStock { get; set; } = 0;

        [Range(0, double.MaxValue)]
        [Display(Name = "Stock minimum")]
        public decimal MinimumStock { get; set; } = 0;

        [Range(0, double.MaxValue)]
        [Display(Name = "Prix de vente")]
        public decimal SellingPrice { get; set; } = 0;

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<ProductionOrder> ProductionOrders { get; set; }
            = new List<ProductionOrder>();

        public ICollection<StockOutput> StockOutputs { get; set; }
            = new List<StockOutput>();

        public ICollection<StockMovement> StockMovements { get; set; }
            = new List<StockMovement>();
    }
}