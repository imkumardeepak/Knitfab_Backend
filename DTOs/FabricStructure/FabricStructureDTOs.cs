using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.FabricStructure
{
    /// <summary>
    /// DTO for FabricStructureMaster data response
    /// </summary>
    public class FabricStructureResponseDto
    {
        public int Id { get; set; }
        public string Fabricstr { get; set; } = string.Empty;
        public decimal Standardeffencny { get; set; }
        public string? FabricCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new FabricStructureMaster
    /// </summary>
    public class CreateFabricStructureRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Fabricstr { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100, ErrorMessage = "Standard efficiency must be between 0.01 and 100")]
        public decimal Standardeffencny { get; set; }

        [MaxLength(50)]
        public string? FabricCode { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing FabricStructureMaster
    /// </summary>
    public class UpdateFabricStructureRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Fabricstr { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100, ErrorMessage = "Standard efficiency must be between 0.01 and 100")]
        public decimal Standardeffencny { get; set; }

        [MaxLength(50)]
        public string? FabricCode { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for FabricStructureMaster search request
    /// </summary>
    public class FabricStructureSearchRequestDto
    {
        [MaxLength(200)]
        public string? Fabricstr { get; set; }

        [MaxLength(50)]
        public string? FabricCode { get; set; }

        public bool? IsActive { get; set; }
    }
}