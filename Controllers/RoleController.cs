using AvyyanBackend.DTOs.Role;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.Filters;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequirePermission("Role Master")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;
        private readonly IAuditLogService _auditLogService;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger, IAuditLogService auditLogService)
        {
            _roleService = roleService;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        /// <summary>Get all roles</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all roles");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Get role by ID</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleResponseDto>> GetRole(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null) return NotFound("Role not found");
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Create new role</summary>
        [HttpPost]
        public async Task<ActionResult<RoleResponseDto>> CreateRole([FromBody] CreateRoleRequestDto createRoleDto)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(createRoleDto);

                await _auditLogService.LogAsync(
                    action: "CREATE",
                    module: "RoleMaster",
                    entityId: role.Id,
                    entityName: role.RoleName,
                    changeSummary: $"Created Role '{role.RoleName}' with {role.PageAccesses?.Count() ?? 0} permissions",
                    newValues: new { role.RoleName, PageAccesses = role.PageAccesses?.Select(p => p.PageName) }
                );

                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Update role</summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<RoleResponseDto>> UpdateRole(int id, [FromBody] UpdateRoleRequestDto updateRoleDto)
        {
            try
            {
                var old = await _roleService.GetRoleByIdAsync(id);
                var role = await _roleService.UpdateRoleAsync(id, updateRoleDto);
                if (role == null) return NotFound("Role not found");

                await _auditLogService.LogAsync(
                    action: "UPDATE",
                    module: "RoleMaster",
                    entityId: id,
                    entityName: role.RoleName,
                    changeSummary: $"Updated Role '{role.RoleName}' — Permissions changed",
                    oldValues: old == null ? null : new { old.RoleName, PageAccesses = old.PageAccesses?.Select(p => p.PageName) },
                    newValues: new { role.RoleName, PageAccesses = role.PageAccesses?.Select(p => p.PageName) }
                );

                return Ok(role);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Delete role</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            try
            {
                var existing = await _roleService.GetRoleByIdAsync(id);
                var result = await _roleService.DeleteRoleAsync(id);
                if (!result) return NotFound("Role not found or cannot delete system role");

                await _auditLogService.LogAsync(
                    action: "DELETE",
                    module: "RoleMaster",
                    entityId: id,
                    entityName: existing?.RoleName,
                    changeSummary: $"Deleted Role '{existing?.RoleName}'",
                    oldValues: existing == null ? null : new { existing.RoleName, PageAccesses = existing.PageAccesses?.Select(p => p.PageName) }
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
