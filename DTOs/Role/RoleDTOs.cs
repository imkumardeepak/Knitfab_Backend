using AvyyanBackend.Models;
using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.Role
{
    /// <summary>
    /// DTO for role response
    /// </summary>
    public class RoleResponseDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IEnumerable<RolePageAccessDto> PageAccesses { get; set; } = new List<RolePageAccessDto>();
    }

    /// <summary>
    /// DTO for creating a new role
    /// </summary>
    public class CreateRoleRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

		public IEnumerable<CreatePageAccessRequestDto> PageAccesses { get; set; } = new List<CreatePageAccessRequestDto>();
    }

    /// <summary>
    /// DTO for updating an existing role
    /// </summary>
    public class UpdateRoleRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

		public IEnumerable<UpdatePageAccessRequestDto> PageAccesses { get; set; } = new List<UpdatePageAccessRequestDto>();
    }

    /// <summary>
    /// DTO for creating page access
    /// </summary>
    public class CreatePageAccessRequestDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string PageName { get; set; } = string.Empty;

        public bool IsView { get; set; } = false;
        public bool IsAdd { get; set; } = false;
        public bool IsEdit { get; set; } = false;
        public bool IsDelete { get; set; } = false;
    }

    /// <summary>
    /// DTO for updating page access
    /// </summary>
    public class UpdatePageAccessRequestDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string PageName { get; set; } = string.Empty;

        public bool IsView { get; set; } = false;
        public bool IsAdd { get; set; } = false;
        public bool IsEdit { get; set; } = false;
        public bool IsDelete { get; set; } = false;
    }

	/// <summary>
	/// DTO for creating a new role
	/// </summary>
	public class CreateRoleDto
	{
		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(500)]
		public string? Description { get; set; }

		public bool IsActive { get; set; } = true;

		public List<PageAccess> PageAccesses { get; set; } = new List<PageAccess>();
	}

	public class UpdateRoleDto
	{
		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(500)]
		public string? Description { get; set; }

		public bool IsActive { get; set; } = true;

		public List<PageAccess> PageAccesses { get; set; } = new List<PageAccess>();
	}

	/// <summary>
	/// DTO for page access response
	/// </summary>
	public class PageAccessResponseDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string PageName { get; set; } = string.Empty;
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

   

    /// <summary>
    /// DTO for page access in role context
    /// </summary>
    public class RolePageAccessDto
    {
        public int Id { get; set; }
        public string PageName { get; set; } = string.Empty;
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
    }
}