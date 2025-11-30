using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class LocationMaster : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Warehousename { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Sublocation { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Locationcode { get; set; } = string.Empty;
    }
}