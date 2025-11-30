using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.ProAllotDto
{
    // Request DTO for creating a roll assignment
    public class CreateRollAssignmentRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Machine allocation ID must be greater than 0")]
        public int MachineAllocationId { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Shift ID must be greater than 0")]
        public int ShiftId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Assigned rolls must be greater than or equal to 0")]
        public decimal AssignedRolls { get; set; }

        [Required(ErrorMessage = "Operator name is required")]
        [StringLength(100, ErrorMessage = "Operator name cannot exceed 100 characters")]
        public string OperatorName { get; set; }
        
        [Required(ErrorMessage = "Timestamp is required")]
        public DateTime Timestamp { get; set; }
    }

    // Request DTO for generating stickers
    public class GenerateStickersRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Roll assignment ID must be greater than 0")]
        public int RollAssignmentId { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Sticker count must be greater than or equal to 0")]
        public decimal StickerCount { get; set; }
    }

    // Request DTO for generating barcodes
    public class GenerateBarcodesRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Roll assignment ID must be greater than 0")]
        public int RollAssignmentId { get; set; }
        
        [Range(1, double.MaxValue, ErrorMessage = "Barcode count must be greater than 0")]
        public decimal BarcodeCount { get; set; }
    }

    // Response DTO for roll assignment
    public class RollAssignmentResponseDto
    {
        public int Id { get; set; }
        public int MachineAllocationId { get; set; }
        public int ShiftId { get; set; }
        public decimal AssignedRolls { get; set; }
        public decimal GeneratedStickers { get; set; }
        public decimal RemainingRolls { get; set; }
        public string OperatorName { get; set; }
        public DateTime Timestamp { get; set; }
        public List<GeneratedBarcodeDto> GeneratedBarcodes { get; set; } = new List<GeneratedBarcodeDto>();
    }

    // DTO for generated barcode
    public class GeneratedBarcodeDto
    {
        public int Id { get; set; }
        public int RollAssignmentId { get; set; }
        public string Barcode { get; set; }
        public int RollNumber { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}