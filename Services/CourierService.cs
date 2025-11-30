using AutoMapper;
using AvyyanBackend.DTOs.Courier;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class CourierService : ICourierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CourierMaster> _courierRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CourierService> _logger;

        public CourierService(
            IUnitOfWork unitOfWork,
            IRepository<CourierMaster> courierRepository,
            IMapper mapper,
            ILogger<CourierService> logger)
        {
            _unitOfWork = unitOfWork;
            _courierRepository = courierRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CourierResponseDto>> GetAllCouriersAsync()
        {
            _logger.LogDebug("Getting all couriers");
            var couriers = await _courierRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {CourierCount} couriers", couriers.Count());
            return _mapper.Map<IEnumerable<CourierResponseDto>>(couriers);
        }

        public async Task<CourierResponseDto?> GetCourierByIdAsync(int id)
        {
            _logger.LogDebug("Getting courier by ID: {CourierId}", id);
            var courier = await _courierRepository.GetByIdAsync(id);
            if (courier == null)
            {
                _logger.LogWarning("Courier {CourierId} not found or inactive", id);
                return null;
            }
            return _mapper.Map<CourierResponseDto>(courier);
        }

        public async Task<CourierResponseDto> CreateCourierAsync(CreateCourierRequestDto createCourierDto)
        {
            _logger.LogDebug("Creating new courier: {CourierName}", createCourierDto.CourierName);

            // Check if courier is unique
            var existingCourier = await _courierRepository.FirstOrDefaultAsync(m => m.CourierName == createCourierDto.CourierName);
            if (existingCourier != null)
            {
                throw new InvalidOperationException($"Courier with name '{createCourierDto.CourierName}' already exists");
            }

            var courier = _mapper.Map<CourierMaster>(createCourierDto);
            await _courierRepository.AddAsync(courier);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created courier {CourierId} with name: {CourierName}", courier.Id, courier.CourierName);
            return _mapper.Map<CourierResponseDto>(courier);
        }

        public async Task<CourierResponseDto?> UpdateCourierAsync(int id, UpdateCourierRequestDto updateCourierDto)
        {
            _logger.LogDebug("Updating courier {CourierId}", id);

            var courier = await _courierRepository.GetByIdAsync(id);
            if (courier == null)
            {
                _logger.LogWarning("Courier {CourierId} not found for update", id);
                return null;
            }

            // Check if courier is unique (excluding current courier)
            var existingCourier = await _courierRepository.FirstOrDefaultAsync(m => 
                m.CourierName == updateCourierDto.CourierName && m.Id != id);
            if (existingCourier != null)
            {
                throw new InvalidOperationException($"Courier with name '{updateCourierDto.CourierName}' already exists");
            }

            _mapper.Map(updateCourierDto, courier);
            courier.UpdatedAt = DateTime.UtcNow;

            _courierRepository.Update(courier);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated courier {CourierId}", id);
            return _mapper.Map<CourierResponseDto>(courier);
        }

        public async Task<bool> DeleteCourierAsync(int id)
        {
            _logger.LogDebug("Deleting courier {CourierId}", id);

            var courier = await _courierRepository.GetByIdAsync(id);
            if (courier == null)
            {
                _logger.LogWarning("Courier {CourierId} not found for deletion", id);
                return false;
            }
            _courierRepository.Remove(courier);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CourierResponseDto>> SearchCouriersAsync(CourierSearchRequestDto searchDto)
        {
            _logger.LogDebug("Searching couriers with name: {CourierName}", searchDto.CourierName);

            var couriers = await _courierRepository.FindAsync(m =>
                m.IsActive &&
                (string.IsNullOrEmpty(searchDto.CourierName) || m.CourierName.Contains(searchDto.CourierName)) &&
                (!searchDto.IsActive.HasValue || m.IsActive == searchDto.IsActive.Value));

            return _mapper.Map<IEnumerable<CourierResponseDto>>(couriers);
        }

        public async Task<bool> IsCourierUniqueAsync(string courierName, int? excludeId = null)
        {
            var query = await _courierRepository.FindAsync(m => m.CourierName == courierName);
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }
            return !query.Any();
        }
    }
}