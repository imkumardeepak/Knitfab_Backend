using AvyyanBackend.DTOs.User;
using AvyyanBackend.DTOs.Auth;

namespace AvyyanBackend.Interfaces
{
    public interface IUserService
    {
        // User Management
        Task<AdminUserResponseDto> CreateUserAsync(CreateUserRequestDto createUserDto);
        Task<UserProfileResponseDto?> GetUserByIdAsync(int userId);
        Task<UserProfileResponseDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<AdminUserResponseDto>> GetAllUsersAsync();
        Task<AdminUserResponseDto?> UpdateUserAsync(int userId, UpdateUserRequestDto updateUserDto);
        Task<bool> DeleteUserAsync(int userId);

        // Profile Management
        Task<UserProfileResponseDto?> UpdateProfileAsync(int userId, UpdateUserProfileRequestDto updateUserDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDto changePasswordDto);

        // User Permissions
        Task<UserPermissionsResponseDto> GetUserPermissionsAsync(int userId);
        Task<bool> HasPageAccessAsync(int userId, string pageName);

        // Validation
        Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null);
        Task<bool> ValidatePasswordAsync(string password);
    }
}