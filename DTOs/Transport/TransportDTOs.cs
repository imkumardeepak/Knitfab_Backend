using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.Transport
{
    /// <summary>
    /// DTO for TransportMaster data response
    /// </summary>
    public class TransportResponseDto
    {
        public int Id { get; set; }
        public string TransportName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Address { get; set; }
        public string? VehicleNumber { get; set; }
        public string? DriverName { get; set; }
        public string? DriverNumber { get; set; }
        public string? LicenseNumber { get; set; }
        public decimal? MaximumCapacityKgs { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new TransportMaster
    /// </summary>
    public class CreateTransportRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string TransportName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? GstNo { get; set; }

        [MaxLength(50)]
        public string? VehicleNumber { get; set; }

        [MaxLength(100)]
        public string? DriverName { get; set; }

        [MaxLength(20)]
        public string? DriverNumber { get; set; }

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Maximum capacity must be greater than 0")]
        public decimal? MaximumCapacityKgs { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing TransportMaster
    /// </summary>
    public class UpdateTransportRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string TransportName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? GstNo { get; set; }

        [MaxLength(50)]
        public string? VehicleNumber { get; set; }

        [MaxLength(100)]
        public string? DriverName { get; set; }

        [MaxLength(20)]
        public string? DriverNumber { get; set; }

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Maximum capacity must be greater than 0")]
        public decimal? MaximumCapacityKgs { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for TransportMaster search request
    /// </summary>
    public class TransportSearchRequestDto
    {
        [MaxLength(200)]
        public string? TransportName { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(50)]
        public string? VehicleNumber { get; set; }

        [MaxLength(100)]
        public string? DriverName { get; set; }

        [MaxLength(20)]
        public string? DriverNumber { get; set; }

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        public bool? IsActive { get; set; }
    }
}