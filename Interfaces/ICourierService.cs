using AvyyanBackend.DTOs.Courier;

namespace AvyyanBackend.Interfaces
{
    public interface ICourierService
    {
        Task<IEnumerable<CourierResponseDto>> GetAllCouriersAsync();
        Task<CourierResponseDto?> GetCourierByIdAsync(int id);
        Task<CourierResponseDto> CreateCourierAsync(CreateCourierRequestDto createCourierDto);
        Task<CourierResponseDto?> UpdateCourierAsync(int id, UpdateCourierRequestDto updateCourierDto);
        Task<bool> DeleteCourierAsync(int id);
        Task<IEnumerable<CourierResponseDto>> SearchCouriersAsync(CourierSearchRequestDto searchDto);
        Task<bool> IsCourierUniqueAsync(string courierName, int? excludeId = null);
    }
}