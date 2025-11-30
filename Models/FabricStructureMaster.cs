using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class FabricStructureMaster : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Fabricstr { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,5)")]
        public decimal Standardeffencny { get; set; }

        [MaxLength(50)]
        public string? FabricCode { get; set; }
    }
}