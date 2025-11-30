using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.DTOs.ProductionConfirmation
{
    public class RollConfirmationRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string AllotId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MachineName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,3)")]
        public decimal RollPerKg { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GreyGsm { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GreyWidth { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal BlendPercent { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Cotton { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Polyester { get; set; }

        [Required]
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

    public class RollConfirmationResponseDto
    {
        public int Id { get; set; }
        public string AllotId { get; set; }
        public string MachineName { get; set; }
        public decimal RollPerKg { get; set; }
        public decimal GreyGsm { get; set; }
        public decimal GreyWidth { get; set; }
        public decimal BlendPercent { get; set; }
        public decimal Cotton { get; set; }
        public decimal Polyester { get; set; }
        public decimal Spandex { get; set; }
        public string RollNo { get; set; }
        
        // Weight fields for FG Sticker Confirmation
        public decimal? GrossWeight { get; set; }
        public decimal? TareWeight { get; set; }
        public decimal? NetWeight { get; set; }
        
        // FG Roll Number - Auto-incremented per AllotId
        public int? FgRollNo { get; set; }
        
        // Flag to indicate if FG Sticker has been generated
        public bool IsFGStickerGenerated { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class RollConfirmationUpdateDto
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal? GrossWeight { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TareWeight { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetWeight { get; set; }
        
        // Fabric specification fields
        [Column(TypeName = "decimal(18,2)")]
        public decimal? GreyGsm { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GreyWidth { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? BlendPercent { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Cotton { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Polyester { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Spandex { get; set; }
        
        // Flag to indicate if FG Sticker has been generated
        public bool? IsFGStickerGenerated { get; set; }
        
        // FG Roll Number - Auto-incremented per AllotId
        public int? FgRollNo { get; set; }
    }
}