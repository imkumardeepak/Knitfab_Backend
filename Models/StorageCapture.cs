using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class StorageCapture : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string LotNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FGRollNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LocationCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Tape { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        public bool IsDispatched { get; set; } = false;
    }
}