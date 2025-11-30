using AvyyanBackend.DTOs.Transport;

namespace AvyyanBackend.Interfaces
{
    public interface ITransportService
    {
        Task<IEnumerable<TransportResponseDto>> GetAllTransportsAsync();
        Task<TransportResponseDto?> GetTransportByIdAsync(int id);
        Task<TransportResponseDto> CreateTransportAsync(CreateTransportRequestDto createTransportDto);
        Task<TransportResponseDto?> UpdateTransportAsync(int id, UpdateTransportRequestDto updateTransportDto);
        Task<bool> DeleteTransportAsync(int id);
        Task<IEnumerable<TransportResponseDto>> SearchTransportsAsync(TransportSearchRequestDto searchDto);
        Task<bool> IsTransportUniqueAsync(string transportName, int? excludeId = null);
    }
}