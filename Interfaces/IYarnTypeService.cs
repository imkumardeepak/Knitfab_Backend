using AvyyanBackend.DTOs.YarnType;

namespace AvyyanBackend.Interfaces
{
    public interface IYarnTypeService
    {
        Task<IEnumerable<YarnTypeResponseDto>> GetAllYarnTypesAsync();
        Task<YarnTypeResponseDto?> GetYarnTypeByIdAsync(int id);
        Task<YarnTypeResponseDto> CreateYarnTypeAsync(CreateYarnTypeRequestDto createYarnTypeDto);
        Task<YarnTypeResponseDto?> UpdateYarnTypeAsync(int id, UpdateYarnTypeRequestDto updateYarnTypeDto);
        Task<bool> DeleteYarnTypeAsync(int id);
        Task<IEnumerable<YarnTypeResponseDto>> SearchYarnTypesAsync(YarnTypeSearchRequestDto searchDto);
        Task<bool> IsYarnTypeUniqueAsync(string yarnType, int? excludeId = null);
    }
}