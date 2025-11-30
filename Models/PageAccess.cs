using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AvyyanBackend.Models
{
    public class PageAccess
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string? PageName { get; set; }
        public bool IsView { get; set; } = false;
        public bool IsAdd { get; set; } = false;
        public bool IsEdit { get; set; } = false;
        public bool IsDelete { get; set; } = false;
		[JsonIgnore]
		public RoleMaster Role { get; set; } = null!;
    }
}