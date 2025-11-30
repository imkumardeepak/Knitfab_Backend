using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class YarnTypeMaster : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string YarnType { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? YarnCode { get; set; }

        [MaxLength(20)]
        [Column("ShortCode")]
        public string? ShortCode { get; set; }
    }
}