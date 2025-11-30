using AvyyanBackend.DTOs.Machine;

namespace AvyyanBackend.Interfaces
{
    public interface IMachineManagerService
    {
        // Basic CRUD Operations
        Task<IEnumerable<MachineResponseDto>> GetAllMachinesAsync();
        Task<MachineResponseDto?> GetMachineByIdAsync(int id);
        Task<MachineResponseDto> CreateMachineAsync(CreateMachineRequestDto createMachineDto);
        Task<MachineResponseDto?> UpdateMachineAsync(int id, UpdateMachineRequestDto updateMachineDto);
        Task<bool> DeleteMachineAsync(int id);

        // Search Operations
        Task<IEnumerable<MachineResponseDto>> SearchMachinesAsync(MachineSearchRequestDto searchDto);

        // Bulk Operations
        Task<IEnumerable<MachineResponseDto>> CreateMultipleMachinesAsync(BulkCreateMachineRequestDto bulkCreateDto);

        // Validation
        Task<bool> IsMachineNameUniqueAsync(string machineName, int? excludeId = null);
    }
}
