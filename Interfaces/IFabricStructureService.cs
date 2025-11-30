using AvyyanBackend.DTOs.FabricStructure;

namespace AvyyanBackend.Interfaces
{
    public interface IFabricStructureService
    {
        Task<IEnumerable<FabricStructureResponseDto>> GetAllFabricStructuresAsync();
        Task<FabricStructureResponseDto?> GetFabricStructureByIdAsync(int id);
        Task<FabricStructureResponseDto> CreateFabricStructureAsync(CreateFabricStructureRequestDto createFabricStructureDto);
        Task<FabricStructureResponseDto?> UpdateFabricStructureAsync(int id, UpdateFabricStructureRequestDto updateFabricStructureDto);
        Task<bool> DeleteFabricStructureAsync(int id);
        Task<IEnumerable<FabricStructureResponseDto>> SearchFabricStructuresAsync(FabricStructureSearchRequestDto searchDto);
        Task<bool> IsFabricStructureUniqueAsync(string fabricStructure, int? excludeId = null);
    }
}