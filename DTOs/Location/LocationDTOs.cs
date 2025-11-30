using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.Location
{
    /// <summary>
    /// DTO for LocationMaster data response
    /// </summary>
    public class LocationResponseDto
    {
        public int Id { get; set; }
        public string Warehousename { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Sublocation { get; set; } = string.Empty;
        public string Locationcode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new LocationMaster
    /// </summary>
    public class CreateLocationRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Warehousename { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Sublocation { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Locationcode { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating an existing LocationMaster
    /// </summary>
    public class UpdateLocationRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Warehousename { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Sublocation { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Locationcode { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for LocationMaster search request
    /// </summary>
    public class LocationSearchRequestDto
    {
        [MaxLength(200)]
        public string? Warehousename { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(200)]
        public string? Sublocation { get; set; }

        [MaxLength(50)]
        public string? Locationcode { get; set; }

        public bool? IsActive { get; set; }
    }
}