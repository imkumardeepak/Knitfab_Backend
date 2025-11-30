using AvyyanBackend.DTOs.Location;

namespace AvyyanBackend.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync();
        Task<LocationResponseDto?> GetLocationByIdAsync(int id);
        Task<LocationResponseDto> CreateLocationAsync(CreateLocationRequestDto createLocationDto);
        Task<LocationResponseDto?> UpdateLocationAsync(int id, UpdateLocationRequestDto updateLocationDto);
        Task<bool> DeleteLocationAsync(int id);
        Task<IEnumerable<LocationResponseDto>> SearchLocationsAsync(LocationSearchRequestDto searchDto);
        Task<bool> IsLocationCodeUniqueAsync(string locationCode, int? excludeId = null);
    }
}