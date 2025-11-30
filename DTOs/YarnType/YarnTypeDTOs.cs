using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.YarnType
{
    /// <summary>
    /// DTO for YarnTypeMaster data response
    /// </summary>
    public class YarnTypeResponseDto
    {
        public int Id { get; set; }
        public string YarnType { get; set; } = string.Empty;
        public string? YarnCode { get; set; }
        public string? ShortCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new YarnTypeMaster
    /// </summary>
    public class CreateYarnTypeRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string YarnType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? YarnCode { get; set; }

        [MaxLength(20)]
        public string? ShortCode { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing YarnTypeMaster
    /// </summary>
    public class UpdateYarnTypeRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string YarnType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? YarnCode { get; set; }

        [MaxLength(20)]
        public string? ShortCode { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for YarnTypeMaster search request
    /// </summary>
    public class YarnTypeSearchRequestDto
    {
        [MaxLength(200)]
        public string? YarnType { get; set; }

        [MaxLength(50)]
        public string? YarnCode { get; set; }

        [MaxLength(20)]
        public string? ShortCode { get; set; }

        public bool? IsActive { get; set; }
    }
}