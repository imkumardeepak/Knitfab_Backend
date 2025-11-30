using AvyyanBackend.DTOs.Auth;
using AvyyanBackend.DTOs.User;

namespace AvyyanBackend.Interfaces
{
    public interface IAuthService
    {
        // Authentication
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto);
        Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenDto);
        Task<bool> LogoutAsync(int userId);

        // Registration
        Task<UserProfileResponseDto> RegisterAsync(RegisterRequestDto registerDto);

        // Password Management
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDto changePasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordDto);
        Task<bool> SetPasswordAsync(SetPasswordRequestDto setPasswordDto);

        // Token Management
        string GenerateJwtToken(AuthUserDto user, IEnumerable<string> roles);
        string GenerateRefreshToken();

        // Authentication Helpers
        Task<bool> ValidatePasswordAsync(string password, string hash);
        string HashPassword(string password);
    }
}