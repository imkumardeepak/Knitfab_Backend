using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.Shift
{
    /// <summary>
    /// DTO for ShiftMaster data response
    /// </summary>
    public class ShiftResponseDto
    {
        public int Id { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int DurationInHours { get; set; }
        // Removed CompanyId field as requested
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new ShiftMaster
    /// </summary>
    public class CreateShiftRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string ShiftName { get; set; } = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int DurationInHours { get; set; }

        // Removed CompanyId field as requested
    }

    /// <summary>
    /// DTO for updating an existing ShiftMaster
    /// </summary>
    public class UpdateShiftRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string ShiftName { get; set; } = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int DurationInHours { get; set; }

        // Removed CompanyId field as requested

        public bool IsActive { get; set; } = true;
    }

    // <summary>
    // DTO for ShiftMaster search request
    // </summary>
    public class ShiftSearchRequestDto
    {
        [MaxLength(100)]
        public string? ShiftName { get; set; }

        public bool? IsActive { get; set; }
    }
}