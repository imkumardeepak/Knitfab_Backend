using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class TapeColorMaster : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string TapeColor { get; set; } = string.Empty;
    }
}