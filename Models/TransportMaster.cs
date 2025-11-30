using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class TransportMaster : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string TransportName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? VehicleNumber { get; set; }

        [MaxLength(100)]
        public string? DriverName { get; set; }

        [MaxLength(20)]
        public string? DriverNumber { get; set; }

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal? MaximumCapacityKgs { get; set; }

        public bool IsActive { get; set; } = true;
    }
}