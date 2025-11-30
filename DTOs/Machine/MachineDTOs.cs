using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.Machine
{
    /// <summary>
    /// DTO for machine data response
    /// </summary>
    public class MachineResponseDto
    {
        public int Id { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public decimal Dia { get; set; }
        public decimal Gg { get; set; }
        public int Needle { get; set; }
        public int Feeder { get; set; }
        public decimal Rpm { get; set; }
        public decimal? Constat { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        // Added MachineType field
        public string? MachineType { get; set; } = "Single Jersey";
    }

    /// <summary>
    /// DTO for creating a new machine
    /// </summary>
    public class CreateMachineRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string MachineName { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Dia must be greater than 0")]
        public decimal Dia { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Gg must be greater than 0")]
        public decimal Gg { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Needle must be greater than 0")]
        public int Needle { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Feeder must be greater than 0")]
        public int Feeder { get; set; }

        [Required]
        [Range(0.00001, double.MaxValue, ErrorMessage = "Rpm must be greater than 0")]
        public decimal Rpm { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Constat must be greater than or equal to 0")]
        public decimal? Constat { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Added MachineType field
        [MaxLength(50)]
        public string? MachineType { get; set; } = "Single Jersey";
    }

    /// <summary>
    /// DTO for updating an existing machine
    /// </summary>
    public class UpdateMachineRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string MachineName { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Dia must be greater than 0")]
        public decimal Dia { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Gg must be greater than 0")]
        public decimal Gg { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Needle must be greater than 0")]
        public int Needle { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Feeder must be greater than 0")]
        public int Feeder { get; set; }

        [Required]
        [Range(0.00001, double.MaxValue, ErrorMessage = "Rpm must be greater than 0")]
        public decimal Rpm { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Constat must be greater than or equal to 0")]
        public decimal? Constat { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        
        // Added MachineType field
        [MaxLength(50)]
        public string? MachineType { get; set; } = "Single Jersey";
    }

    /// <summary>
    /// DTO for machine search request
    /// </summary>
    public class MachineSearchRequestDto
    {
        [MaxLength(200)]
        public string? MachineName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Dia must be greater than 0")]
        public decimal? Dia { get; set; }

        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO for bulk machine creation request
    /// </summary>
    public class BulkCreateMachineRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one machine must be provided")]
        public IEnumerable<CreateMachineRequestDto> Machines { get; set; } = new List<CreateMachineRequestDto>();
    }
}