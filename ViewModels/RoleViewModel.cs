using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.ViewModels
{
    public class RoleViewModel
    {
        public string? Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Nom du rôle")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}