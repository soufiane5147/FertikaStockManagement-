using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Nom du fournisseur")]
        public string Name { get; set; } = string.Empty;

        [StringLength(150)]
        [Display(Name = "Personne de contact")]
        public string? ContactPerson { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(30)]
        public string? Phone { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<RawMaterial> RawMaterials { get; set; }
            = new List<RawMaterial>();

        public ICollection<StockEntry> StockEntries { get; set; }
            = new List<StockEntry>();
    }
}