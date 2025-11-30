using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvyyanBackend.DTOs.User;
using AvyyanBackend.DTOs.Auth;
using AvyyanBackend.Interfaces;
using System.Security.Claims;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileResponseDto>> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user profile");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        [HttpPut("profile")]
        public async Task<ActionResult<UserProfileResponseDto>> UpdateProfile(UpdateUserProfileRequestDto updateUserDto)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var user = await _userService.UpdateProfileAsync(userId.Value, updateUserDto);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user profile");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Change current user password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequestDto changePasswordDto)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var result = await _userService.ChangePasswordAsync(userId.Value, changePasswordDto);
                if (!result)
                    return BadRequest("Current password is incorrect");

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get user's page access permissions
        /// </summary>
        [HttpGet("permissions")]
        public async Task<ActionResult<UserPermissionsResponseDto>> GetPermissions()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var permissions = await _userService.GetUserPermissionsAsync(userId.Value);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user permissions");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }



        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
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

        /// <summary>
        /// Create new user (Admin only)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AdminUserResponseDto>> CreateUser(CreateUserRequestDto createUserDto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminUserResponseDto>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update user (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<AdminUserResponseDto>> UpdateUser(int id, UpdateUserRequestDto updateUserDto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound($"User with ID {id} not found");
                }
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