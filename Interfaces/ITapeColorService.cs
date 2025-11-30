using AvyyanBackend.DTOs.TapeColor;

namespace AvyyanBackend.Interfaces
{
    public interface ITapeColorService
    {
        Task<IEnumerable<TapeColorResponseDto>> GetAllTapeColorsAsync();
        Task<TapeColorResponseDto?> GetTapeColorByIdAsync(int id);
        Task<TapeColorResponseDto> CreateTapeColorAsync(CreateTapeColorRequestDto createTapeColorDto);
        Task<TapeColorResponseDto?> UpdateTapeColorAsync(int id, UpdateTapeColorRequestDto updateTapeColorDto);
        Task<bool> DeleteTapeColorAsync(int id);
        Task<IEnumerable<TapeColorResponseDto>> SearchTapeColorsAsync(TapeColorSearchRequestDto searchDto);
        Task<bool> IsTapeColorUniqueAsync(string tapeColor, int? excludeId = null);
        // New method to check if tape color is already assigned to a lotment
        Task<bool> IsTapeColorAssignedToLotmentAsync(string tapeColor, string lotmentId);
    }
}