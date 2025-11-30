using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
		[Column(TypeName = "timestamp without time zone")]
		public DateTime? UpdatedAt { get; set; }= DateTime.Now;

		public bool IsActive { get; set; } = true;
        
        public string? CreatedBy { get; set; }
        
        public string? UpdatedBy { get; set; }
    }
}
