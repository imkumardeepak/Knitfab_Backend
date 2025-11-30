using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.TapeColor
{
    /// <summary>
    /// DTO for TapeColorMaster data response
    /// </summary>
    public class TapeColorResponseDto
    {
        public int Id { get; set; }
        public string TapeColor { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new TapeColorMaster
    /// </summary>
    public class CreateTapeColorRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string TapeColor { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating an existing TapeColorMaster
    /// </summary>
    public class UpdateTapeColorRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string TapeColor { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for TapeColorMaster search request
    /// </summary>
    public class TapeColorSearchRequestDto
    {
        [MaxLength(200)]
        public string? TapeColor { get; set; }

        public bool? IsActive { get; set; }
    }
}