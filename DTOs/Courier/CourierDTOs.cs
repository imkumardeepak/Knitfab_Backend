using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.Courier
{
    /// <summary>
    /// DTO for CourierMaster data response
    /// </summary>
    public class CourierResponseDto
    {
        public int Id { get; set; }
        public string CourierName { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? GstNo { get; set; }
        public string? TrackingUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new CourierMaster
    /// </summary>
    public class CreateCourierRequestDto
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
    }

    /// <summary>
    /// DTO for updating an existing CourierMaster
    /// </summary>
    public class UpdateCourierRequestDto
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

    /// <summary>
    /// DTO for CourierMaster search request
    /// </summary>
    public class CourierSearchRequestDto
    {
        [MaxLength(200)]
        public string? CourierName { get; set; }

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        public bool? IsActive { get; set; }
    }
}