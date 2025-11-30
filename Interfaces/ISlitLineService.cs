using AvyyanBackend.DTOs.SlitLine;

namespace AvyyanBackend.Interfaces
{
    public interface ISlitLineService
    {
        Task<IEnumerable<SlitLineResponseDto>> GetAllSlitLinesAsync();
        Task<SlitLineResponseDto?> GetSlitLineByIdAsync(int id);
        Task<SlitLineResponseDto> CreateSlitLineAsync(CreateSlitLineRequestDto createSlitLineDto);
        Task<SlitLineResponseDto?> UpdateSlitLineAsync(int id, UpdateSlitLineRequestDto updateSlitLineDto);
        Task<bool> DeleteSlitLineAsync(int id);
        Task<IEnumerable<SlitLineResponseDto>> SearchSlitLinesAsync(SlitLineSearchRequestDto searchDto);
    }
}