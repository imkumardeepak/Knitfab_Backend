using AvyyanBackend.DTOs.Shift;

namespace AvyyanBackend.Interfaces
{
    public interface IShiftService
    {
        Task<IEnumerable<ShiftResponseDto>> GetAllShiftsAsync();
        Task<ShiftResponseDto?> GetShiftByIdAsync(int id);
        Task<ShiftResponseDto> CreateShiftAsync(CreateShiftRequestDto createShiftDto);
        Task<ShiftResponseDto?> UpdateShiftAsync(int id, UpdateShiftRequestDto updateShiftDto);
        Task<bool> DeleteShiftAsync(int id);
        Task<IEnumerable<ShiftResponseDto>> SearchShiftsAsync(ShiftSearchRequestDto searchDto);
        Task<bool> IsShiftNameUniqueAsync(string shiftName, int? excludeId = null);
    }
}