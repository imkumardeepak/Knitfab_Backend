using AvyyanBackend.DTOs.Role;

namespace AvyyanBackend.Interfaces
{
    public interface IRoleService
    {
        // Role Management
        Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync();
        Task<RoleResponseDto?> GetRoleByIdAsync(int roleId);
        Task<RoleResponseDto> CreateRoleAsync(CreateRoleRequestDto createRoleDto);
        Task<RoleResponseDto?> UpdateRoleAsync(int roleId, UpdateRoleRequestDto updateRoleDto);
        Task<bool> DeleteRoleAsync(int roleId);

        // Validation
        Task<bool> IsRoleNameUniqueAsync(string name, int? excludeRoleId = null);
    }
}