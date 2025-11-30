using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.Role;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Services
{
	public class RoleService : IRoleService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRepository<RoleMaster> _roleRepository;
		private readonly IRepository<User> _userRepository;
		private readonly IRepository<PageAccess> _pageAccessRepository;
		private readonly IMapper _mapper;
		private readonly ILogger<RoleService> _logger;
		protected readonly ApplicationDbContext _context;

		public RoleService(
			IUnitOfWork unitOfWork,
			IRepository<RoleMaster> roleRepository,
			IRepository<User> userRepository,
			IRepository<PageAccess> pageAccessRepository,
			IMapper mapper,
			ILogger<RoleService> logger,
			ApplicationDbContext context)
		{
			_unitOfWork = unitOfWork;
			_roleRepository = roleRepository;
			_userRepository = userRepository;
			_pageAccessRepository = pageAccessRepository;
			_mapper = mapper;
			_logger = logger;
			_context = context;
		}

		public async Task<IEnumerable<RoleResponseDto>> GetAllRolesAsync()
		{
			var roles = await _context.RoleMasters.Include(a => a.PageAccesses).ToListAsync();
			return _mapper.Map<IEnumerable<RoleResponseDto>>(roles);
		}

		public async Task<RoleResponseDto?> GetRoleByIdAsync(int roleId)
		{
			var role = await _context.RoleMasters.Include(a => a.PageAccesses).Where(a => a.Id == roleId).FirstOrDefaultAsync();
			return role != null ? _mapper.Map<RoleResponseDto>(role) : null;
		}

		public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleRequestDto createRoleDto)
		{
			if (!await IsRoleNameUniqueAsync(createRoleDto.Name))
				throw new InvalidOperationException("Role already exists");

			var role = _mapper.Map<RoleMaster>(createRoleDto);

			await _roleRepository.AddAsync(role);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<RoleResponseDto>(role);
		}

		public async Task<RoleResponseDto?> UpdateRoleAsync(int roleId, UpdateRoleRequestDto updateRoleDto)
		{
			var role = await _context.RoleMasters
				.Include(r => r.PageAccesses)
				.FirstOrDefaultAsync(r => r.Id == roleId);

			if (role == null) return null;

			// ✅ Check role name uniqueness
			if (role.RoleName != updateRoleDto.Name &&
				!await IsRoleNameUniqueAsync(updateRoleDto.Name, roleId))
			{
				throw new InvalidOperationException("Role name already exists");
			}

			// ✅ Remove old page accesses
			_context.PageAccesses.RemoveRange(role.PageAccesses);

			// ✅ Add new page accesses (ensure distinct)
			var distinctPageAccesses = updateRoleDto.PageAccesses
				.GroupBy(p => p.PageName)
				.Select(g => g.First())
				.Select(pa => new PageAccess
				{
					PageName = pa.PageName,
					IsView = pa.IsView,
					IsEdit = pa.IsEdit,
					IsAdd = pa.IsAdd,
					IsDelete = pa.IsDelete,
					RoleId = role.Id
				}).ToList();

			role.PageAccesses = distinctPageAccesses;

			// ✅ Update role info
			role.RoleName = updateRoleDto.Name;
			role.Description = updateRoleDto.Description;
			role.IsActive = updateRoleDto.IsActive;

			_roleRepository.Update(role);
			await _unitOfWork.SaveChangesAsync();

			return _mapper.Map<RoleResponseDto>(role);
		}

		public async Task<bool> DeleteRoleAsync(int roleId)
		{
			var role = await _roleRepository.GetByIdAsync(roleId);
			if (role == null) return false;

			_roleRepository.Remove(role);
			return await _unitOfWork.SaveChangesAsync() > 0;
		}
		public async Task<bool> IsRoleNameUniqueAsync(string name, int? excludeRoleId = null)
		{
			var query = await _roleRepository.FindAsync(r => r.RoleName == name);
			if (excludeRoleId.HasValue)
			{
				query = query.Where(r => r.Id != excludeRoleId.Value);
			}
			return !query.Any();
		}
	}
}