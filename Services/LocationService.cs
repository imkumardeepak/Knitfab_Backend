using AutoMapper;
using AvyyanBackend.DTOs.Location;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<LocationMaster> _locationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationService> _logger;

        public LocationService(
            IUnitOfWork unitOfWork,
            IRepository<LocationMaster> locationRepository,
            IMapper mapper,
            ILogger<LocationService> logger)
        {
            _unitOfWork = unitOfWork;
            _locationRepository = locationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync()
        {
            _logger.LogDebug("Getting all locations");
            var locations = await _locationRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {LocationCount} locations", locations.Count());
            return _mapper.Map<IEnumerable<LocationResponseDto>>(locations);
        }

        public async Task<LocationResponseDto?> GetLocationByIdAsync(int id)
        {
            _logger.LogDebug("Getting location by ID: {LocationId}", id);
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
            {
                _logger.LogWarning("Location {LocationId} not found or inactive", id);
                return null;
            }
            return _mapper.Map<LocationResponseDto>(location);
        }

        public async Task<LocationResponseDto> CreateLocationAsync(CreateLocationRequestDto createLocationDto)
        {
            _logger.LogDebug("Creating new location: {LocationCode}", createLocationDto.Locationcode);

            // Check if location code is unique
            var existingLocation = await _locationRepository.FirstOrDefaultAsync(m => m.Locationcode == createLocationDto.Locationcode);
            if (existingLocation != null)
            {
                throw new InvalidOperationException($"Location with code '{createLocationDto.Locationcode}' already exists");
            }

            var location = _mapper.Map<LocationMaster>(createLocationDto);
            await _locationRepository.AddAsync(location);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created location {LocationId} with code: {LocationCode}", location.Id, location.Locationcode);
            return _mapper.Map<LocationResponseDto>(location);
        }

        public async Task<LocationResponseDto?> UpdateLocationAsync(int id, UpdateLocationRequestDto updateLocationDto)
        {
            _logger.LogDebug("Updating location {LocationId}", id);

            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
            {
                _logger.LogWarning("Location {LocationId} not found for update", id);
                return null;
            }

            // Check if location code is unique (excluding current location)
            var existingLocation = await _locationRepository.FirstOrDefaultAsync(m => 
                m.Locationcode == updateLocationDto.Locationcode && m.Id != id);
            if (existingLocation != null)
            {
                throw new InvalidOperationException($"Location with code '{updateLocationDto.Locationcode}' already exists");
            }

            _mapper.Map(updateLocationDto, location);
            location.UpdatedAt = DateTime.Now;

            _locationRepository.Update(location);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated location {LocationId}", id);
            return _mapper.Map<LocationResponseDto>(location);
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            _logger.LogDebug("Deleting location {LocationId}", id);

            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
            {
                _logger.LogWarning("Location {LocationId} not found for deletion", id);
                return false;
            }
            _locationRepository.Remove(location);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LocationResponseDto>> SearchLocationsAsync(LocationSearchRequestDto searchDto)
        {
            _logger.LogDebug("Searching locations with warehouse: {Warehouse}, location: {Location}, sublocation: {Sublocation}", 
                searchDto.Warehousename, searchDto.Location, searchDto.Sublocation);

            var locations = await _locationRepository.FindAsync(m =>
                m.IsActive &&
                (string.IsNullOrEmpty(searchDto.Warehousename) || m.Warehousename.Contains(searchDto.Warehousename)) &&
                (string.IsNullOrEmpty(searchDto.Location) || m.Location.Contains(searchDto.Location)) &&
                (string.IsNullOrEmpty(searchDto.Sublocation) || m.Sublocation.Contains(searchDto.Sublocation)) &&
                (string.IsNullOrEmpty(searchDto.Locationcode) || m.Locationcode.Contains(searchDto.Locationcode)) &&
                (!searchDto.IsActive.HasValue || m.IsActive == searchDto.IsActive.Value));

            return _mapper.Map<IEnumerable<LocationResponseDto>>(locations);
        }

        public async Task<bool> IsLocationCodeUniqueAsync(string locationCode, int? excludeId = null)
        {
            var query = await _locationRepository.FindAsync(m => m.Locationcode == locationCode);
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }
            return !query.Any();
        }
    }
}