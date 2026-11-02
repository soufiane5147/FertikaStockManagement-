using System.ComponentModel.DataAnnotations;

namespace FerticaStockManagement.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [StringLength(100)]
        public string? EntityName { get; set; }

        public string? EntityId { get; set; }

        public string? UserId { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? IpAddress { get; set; }
    }
}