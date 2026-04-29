using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.Filters;
using Microsoft.AspNetCore.Authorization;
using AvyyanBackend.DTOs.User;
using AvyyanBackend.DTOs.Auth;
using AvyyanBackend.Interfaces;
using System.Security.Claims;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequirePermission("User Management")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IAuditLogService _auditLogService;

        public UserController(IUserService userService, ILogger<UserController> logger, IAuditLogService auditLogService)
        {
            _userService = userService;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        /// <summary>Get current user profile</summary>
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileResponseDto>> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return Unauthorized();
                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null) return NotFound("User not found");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user profile");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Update current user profile</summary>
        [HttpPut("profile")]
        public async Task<ActionResult<UserProfileResponseDto>> UpdateProfile(UpdateUserProfileRequestDto updateUserDto)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return Unauthorized();

                var user = await _userService.UpdateProfileAsync(userId.Value, updateUserDto);
                if (user == null) return NotFound("User not found");

                await _auditLogService.LogAsync(
                    action: "UPDATE",
                    module: "UserManagement",
                    entityId: userId.Value,
                    entityName: $"{user.FirstName} {user.LastName}",
                    changeSummary: $"User updated own profile: {user.FirstName} {user.LastName}"
                );

                return Ok(user);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user profile");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Change current user password</summary>
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequestDto changePasswordDto)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return Unauthorized();

                var result = await _userService.ChangePasswordAsync(userId.Value, changePasswordDto);
                if (!result) return BadRequest("Current password is incorrect");

                await _auditLogService.LogAsync(
                    action: "CHANGE_PASSWORD",
                    module: "UserManagement",
                    entityId: userId.Value,
                    changeSummary: $"User #{userId.Value} changed their own password"
                );

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Get user's page access permissions</summary>
        [HttpGet("permissions")]
        public async Task<ActionResult<UserPermissionsResponseDto>> GetPermissions()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return Unauthorized();
                var permissions = await _userService.GetUserPermissionsAsync(userId.Value);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user permissions");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Get all users (Admin only)</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminUserResponseDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Create new user (Admin only)</summary>
        [HttpPost]
        public async Task<ActionResult<AdminUserResponseDto>> CreateUser(CreateUserRequestDto createUserDto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);

                await _auditLogService.LogAsync(
                    action: "CREATE",
                    module: "UserManagement",
                    entityId: user.Id,
                    entityName: $"{user.FirstName} {user.LastName}",
                    changeSummary: $"Created user '{user.FirstName} {user.LastName}' ({user.Email}) with role {user.RoleName}",
                    newValues: new { user.FirstName, user.LastName, user.Email, user.RoleName, user.IsActive }
                );

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Get user by ID (Admin only)</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminUserResponseDto>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound("User not found");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Update user (Admin only)</summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<AdminUserResponseDto>> UpdateUser(int id, UpdateUserRequestDto updateUserDto)
        {
            try
            {
                var old = await _userService.GetUserByIdAsync(id);
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                if (user == null) return NotFound("User not found");

                await _auditLogService.LogAsync(
                    action: "UPDATE",
                    module: "UserManagement",
                    entityId: id,
                    entityName: $"{user.FirstName} {user.LastName}",
                    changeSummary: $"Updated user '{user.FirstName} {user.LastName}' — Role: {old?.RoleName} → {user.RoleName}, Active: {old?.IsActive} → {user.IsActive}",
                    oldValues: old == null ? null : new { old.FirstName, old.LastName, old.Email, old.RoleName, old.IsActive },
                    newValues: new { user.FirstName, user.LastName, user.Email, user.RoleName, user.IsActive }
                );

                return Ok(user);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Delete a user</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var existing = await _userService.GetUserByIdAsync(id);
                var result = await _userService.DeleteUserAsync(id);
                if (!result) return NotFound($"User with ID {id} not found");

                await _auditLogService.LogAsync(
                    action: "DELETE",
                    module: "UserManagement",
                    entityId: id,
                    entityName: existing == null ? null : $"{existing.FirstName} {existing.LastName}",
                    changeSummary: $"Deleted user '{existing?.FirstName} {existing?.LastName}' ({existing?.Email})",
                    oldValues: existing == null ? null : new { existing.FirstName, existing.LastName, existing.Email, existing.RoleName }
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user {UserId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
