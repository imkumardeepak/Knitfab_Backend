using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models.ProAllot
{
    public class GeneratedBarcode
    {
        public int Id { get; set; }

        // Foreign key to RollAssignment
        public int RollAssignmentId { get; set; }

        [Required]
        public string Barcode { get; set; }

        [Required]
        public int RollNumber { get; set; }

        [Required]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual RollAssignment RollAssignment { get; set; }
    }
}