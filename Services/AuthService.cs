using AutoMapper;
using AvyyanBackend.DTOs.Auth;
using AvyyanBackend.DTOs.User;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AvyyanBackend.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRepository<User> _userRepository;
		private readonly IRepository<RoleMaster> _roleRepository;
		private readonly IUserService _userService;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AuthService> _logger;
		private static readonly Dictionary<string, RefreshTokenInfo> _refreshTokens = new();

		public AuthService(
			IUnitOfWork unitOfWork,
			IRepository<User> userRepository,
			IRepository<RoleMaster> roleRepository,
			IUserService userService,
			IMapper mapper,
			IConfiguration configuration,
			ILogger<AuthService> logger)
		{
			_unitOfWork = unitOfWork;
			_userRepository = userRepository;
			_roleRepository = roleRepository;
			_userService = userService;
			_mapper = mapper;
			_configuration = configuration;
			_logger = logger;
		}

		public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto)
		{
			_logger.LogDebug("Attempting login for email: {Email}", loginDto.Email);

			var user = await _userService.GetUserByEmailAsync(loginDto.Email);
			if (user == null)
			{
				_logger.LogWarning("Login failed: User not found for email: {Email}", loginDto.Email);
				return null;
			}

			var userEntity = await _userRepository.FirstOrDefaultAsync(u => u.Id == user.Id);
			if (userEntity == null) return null;

			if (!await ValidatePasswordAsync(loginDto.Password, userEntity.PasswordHash))
			{
				_logger.LogWarning("Login failed: Invalid password for email: {Email}", loginDto.Email);
				return null;
			}

			userEntity.LastLoginAt = DateTime.UtcNow;
			_userRepository.Update(userEntity);
			await _unitOfWork.SaveChangesAsync();

			var permissions = await _userService.GetUserPermissionsAsync(user.Id);
			var authUser = _mapper.Map<AuthUserDto>(userEntity);

			var roles = new List<string> { user.RoleName };
			var token = GenerateJwtToken(authUser, roles);
			var refreshToken = GenerateRefreshToken();

			// Store refresh token with user info
			_refreshTokens[refreshToken] = new RefreshTokenInfo
			{
				UserId = user.Id,
				Email = user.Email,
				FullName = $"{user.FirstName} {user.LastName}",
				RoleName = user.RoleName,
				CreatedAt = DateTime.UtcNow,
				ExpiresAt = DateTime.UtcNow.AddDays(1) // Refresh token valid for 7 days
			};

			_logger.LogInformation("User {Email} logged in successfully", loginDto.Email);

			return new LoginResponseDto
			{
				Token = token,
				RefreshToken = refreshToken,
				ExpiresAt = DateTime.UtcNow.AddDays(1),
				User = authUser,
				Roles = roles,
				PageAccesses = _mapper.Map<IEnumerable<AuthPageAccessDto>>(permissions.PageAccesses)
			};
		}

		public async Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenDto)
		{
			_logger.LogDebug("Attempting to refresh token");

			// Check if refresh token exists and is valid
			if (!_refreshTokens.TryGetValue(refreshTokenDto.RefreshToken, out var refreshTokenInfo))
			{
				_logger.LogWarning("Refresh token not found or invalid");
				return null;
			}

			// Check if refresh token is expired
			if (refreshTokenInfo.ExpiresAt < DateTime.UtcNow)
			{
				_logger.LogWarning("Refresh token expired");
				// Remove expired token
				_refreshTokens.Remove(refreshTokenDto.RefreshToken);
				return null;
			}

			// Get user details
			var user = await _userService.GetUserByIdAsync(refreshTokenInfo.UserId);
			if (user == null)
			{
				_logger.LogWarning("User not found for refresh token");
				return null;
			}

			var userEntity = await _userRepository.FirstOrDefaultAsync(u => u.Id == user.Id);
			if (userEntity == null) return null;

			var permissions = await _userService.GetUserPermissionsAsync(user.Id);
			var authUser = _mapper.Map<AuthUserDto>(userEntity);

			var roles = new List<string> { user.RoleName };
			var token = GenerateJwtToken(authUser, roles);

			// Generate new refresh token
			var newRefreshToken = GenerateRefreshToken();

			// Remove old refresh token and add new one
			_refreshTokens.Remove(refreshTokenDto.RefreshToken);
			_refreshTokens[newRefreshToken] = new RefreshTokenInfo
			{
				UserId = user.Id,
				Email = user.Email,
				FullName = $"{user.FirstName} {user.LastName}",
				RoleName = user.RoleName,
				CreatedAt = DateTime.UtcNow,
				ExpiresAt = DateTime.UtcNow.AddDays(1)
			};

			_logger.LogInformation("Token refreshed successfully for user {Email}", user.Email);

			return new LoginResponseDto
			{
				Token = token,
				RefreshToken = newRefreshToken,
				ExpiresAt = DateTime.UtcNow.AddDays(1),
				User = authUser,
				Roles = roles,
				PageAccesses = _mapper.Map<IEnumerable<AuthPageAccessDto>>(permissions.PageAccesses)
			};
		}

		public async Task<bool> LogoutAsync(int userId)
		{
			// Remove all refresh tokens for this user
			var tokensToRemove = _refreshTokens
				.Where(kv => kv.Value.UserId == userId)
				.Select(kv => kv.Key)
				.ToList();

			foreach (var token in tokensToRemove)
			{
				_refreshTokens.Remove(token);
			}

			// Nothing to do here anymore as we don't handle refresh tokens
			return await Task.FromResult(true);
		}

		public async Task<UserProfileResponseDto> RegisterAsync(RegisterRequestDto registerDto)
		{
			if (!await _userService.IsEmailUniqueAsync(registerDto.Email))
				throw new InvalidOperationException("Email already exists");

			var user = new User
			{
				FirstName = registerDto.FirstName,
				LastName = registerDto.LastName,
				Email = registerDto.Email,
				PasswordHash = HashPassword(registerDto.Password),
				PhoneNumber = registerDto.PhoneNumber,
				RoleName = "User" // Assign a default role
			};

			await _userRepository.AddAsync(user);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<UserProfileResponseDto>(user);
		}

		public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDto changePasswordDto)
		{
			return await _userService.ChangePasswordAsync(userId, changePasswordDto);
		}

		public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordDto)
		{
			var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == resetPasswordDto.Email && u.IsActive);
			if (user == null) return false;

			_logger.LogInformation("Password reset requested for user: {Email}", resetPasswordDto.Email);
			return true;
		}

		public Task<bool> SetPasswordAsync(SetPasswordRequestDto setPasswordDto)
		{
			return Task.FromResult(true);
		}

		public string GenerateJwtToken(AuthUserDto user, IEnumerable<string> roles)
		{
			var jwtSettings = _configuration.GetSection("JwtSettings");
			var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "YourSecretKeyHere");

			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new(ClaimTypes.Email, user.Email),
				new("FullName", $"{user.FirstName} {user.LastName}")
			};

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
				Issuer = jwtSettings["Issuer"],
				Audience = jwtSettings["Audience"]
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		public Task<bool> ValidatePasswordAsync(string password, string hash)
		{
			return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
		}

		public string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}
	}

	// Helper class to store refresh token information
	public class RefreshTokenInfo
	{
		public int UserId { get; set; }
		public string Email { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string RoleName { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public DateTime ExpiresAt { get; set; }
	}
}