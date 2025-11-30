using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class ShiftMaster : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string ShiftName { get; set; } = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int DurationInHours { get; set; }


    }
}