using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class DispatchedRoll : BaseEntity
    {
        public int DispatchPlanningId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string LotNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FGRollNo { get; set; } = string.Empty;

        public bool IsLoaded { get; set; } = false;

        public DateTime? LoadedAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string LoadedBy { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey("DispatchPlanningId")]
        public virtual DispatchPlanning? DispatchPlanning { get; set; }
    }
}