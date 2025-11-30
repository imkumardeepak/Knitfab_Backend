using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class CourierMaster : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string CourierName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? GstNo { get; set; }

        [MaxLength(50)]
        public string? TrackingUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}