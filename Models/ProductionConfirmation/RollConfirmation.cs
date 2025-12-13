using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models.ProductionConfirmation
{
    public class RollConfirmation 
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string AllotId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MachineName { get; set; }

        // Removed MachineRollNo field since we'll use RollNo for this purpose

        [Column(TypeName = "decimal(18,3)")]
        public decimal RollPerKg { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GreyGsm { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GreyWidth { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal BlendPercent { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Cotton { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Polyester { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Spandex { get; set; }

        [Required]
        [MaxLength(50)]
        public string RollNo { get; set; }

        // Weight fields for FG Sticker Confirmation
        [Column(TypeName = "decimal(18,2)")]
        public decimal? GrossWeight { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TareWeight { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetWeight { get; set; }

        // FG Roll Number - Auto-incremented per AllotId
        public int? FgRollNo { get; set; }

        // Flag to indicate if FG Sticker has been generated
        public bool IsFGStickerGenerated { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}