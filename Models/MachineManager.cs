using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class MachineManager : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string MachineName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Dia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Gg { get; set; }

        public int Needle { get; set; }

        public int Feeder { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal Rpm { get; set; }

		[Column(TypeName = "decimal(18,5)")]
		public decimal? Constat { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
      
        // Added MachineType field
        [MaxLength(50)]
        public string? MachineType { get; set; } = "Single Jersey";
    }
}