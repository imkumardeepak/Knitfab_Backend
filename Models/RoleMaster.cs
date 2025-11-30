using System.Collections.Generic;

namespace AvyyanBackend.Models
{
    public class RoleMaster
    {
        public int Id { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<PageAccess> PageAccesses { get; set; } = new List<PageAccess>();
    }
}