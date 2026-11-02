using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Nom")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Téléphone")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Rôle")]
        public string? Role { get; set; }

        public bool IsActive { get; set; } = true;
    }
}