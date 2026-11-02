using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.ViewModels
{
    public class SupplierViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du fournisseur est obligatoire.")]
        [StringLength(150)]
        [Display(Name = "Nom du fournisseur")]
        public string Name { get; set; } = string.Empty;

        [StringLength(150)]
        [Display(Name = "Personne de contact")]
        public string? ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Email invalide.")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
        [StringLength(30)]
        public string? Phone { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public bool IsActive { get; set; } = true;
    }
}