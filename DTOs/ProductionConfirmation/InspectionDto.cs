using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.DTOs.ProductionConfirmation
{
    public class InspectionRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string AllotId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string MachineName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string RollNo { get; set; } = string.Empty;

        // Spinning Faults
        public int ThinPlaces { get; set; }
        public int ThickPlaces { get; set; }
        public int ThinLines { get; set; }
        public int ThickLines { get; set; }
        public int DoubleParallelYarn { get; set; }

        // Contamination Faults
        public int HaidJute { get; set; }
        public int ColourFabric { get; set; }

        // Column 3 Faults
        public int Holes { get; set; }
        public int DropStitch { get; set; }
        public int LycraStitch { get; set; }
        public int LycraBreak { get; set; }
        public int FFD { get; set; }
        public int NeedleBroken { get; set; }
        public int KnitFly { get; set; }
        public int OilSpots { get; set; }
        public int OilLines { get; set; }
        public int VerticalLines { get; set; }

        // Summary
        [Required]
        [MaxLength(10)]
        public string Grade { get; set; } = string.Empty;

        public int TotalFaults { get; set; }

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Flag for approval status (true = approved, false = rejected)
        public bool Flag { get; set; } = true; // Default to approved
    }

    public class InspectionResponseDto
    {
        public int Id { get; set; }
        public string AllotId { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string RollNo { get; set; } = string.Empty;

        // Spinning Faults
        public int ThinPlaces { get; set; }
        public int ThickPlaces { get; set; }
        public int ThinLines { get; set; }
        public int ThickLines { get; set; }
        public int DoubleParallelYarn { get; set; }

        // Contamination Faults
        public int HaidJute { get; set; }
        public int ColourFabric { get; set; }

        // Column 3 Faults
        public int Holes { get; set; }
        public int DropStitch { get; set; }
        public int LycraStitch { get; set; }
        public int LycraBreak { get; set; }
        public int FFD { get; set; }
        public int NeedleBroken { get; set; }
        public int KnitFly { get; set; }
        public int OilSpots { get; set; }
        public int OilLines { get; set; }
        public int VerticalLines { get; set; }

        // Summary
        public string Grade { get; set; } = string.Empty;
        public int TotalFaults { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        
        // Flag for approval status (true = approved, false = rejected)
        public bool Flag { get; set; }
    }
}