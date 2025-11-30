using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models.ProAllot
{
    public class RollAssignment
    {
        public int Id { get; set; }

        // Foreign key to MachineAllocation
        public int MachineAllocationId { get; set; }
        
        [Required]
        public int ShiftId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal AssignedRolls { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal GeneratedStickers { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal RemainingRolls { get; set; }

        [Required]
        public string OperatorName { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual MachineAllocation MachineAllocation { get; set; }
        
        // Navigation property for generated barcodes
        public virtual ICollection<GeneratedBarcode> GeneratedBarcodes { get; set; } = new List<GeneratedBarcode>();
    }
}