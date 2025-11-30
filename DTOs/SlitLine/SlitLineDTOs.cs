using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.SlitLine
{
    /// <summary>
    /// DTO for SlitLineMaster data response
    /// </summary>
    public class SlitLineResponseDto
    {
        public int Id { get; set; }
        public string SlitLine { get; set; } = string.Empty;
        public char SlitLineCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new SlitLineMaster
    /// </summary>
    public class CreateSlitLineRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string SlitLine { get; set; } = string.Empty;

        [Required]
        public char SlitLineCode { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing SlitLineMaster
    /// </summary>
    public class UpdateSlitLineRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string SlitLine { get; set; } = string.Empty;

        [Required]
        public char SlitLineCode { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for SlitLineMaster search request
    /// </summary>
    public class SlitLineSearchRequestDto
    {
        [MaxLength(200)]
        public string? SlitLine { get; set; }

        public char? SlitLineCode { get; set; }

        public bool? IsActive { get; set; }
    }
}