using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class SlitLineMaster : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string SlitLine { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "char(1)")]
        public char SlitLineCode { get; set; }
        
        // Removed SlitLineType field as requested
    }
}