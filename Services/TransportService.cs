using AutoMapper;
using AvyyanBackend.DTOs.Transport;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class TransportService : ITransportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TransportMaster> _transportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TransportService> _logger;

        public TransportService(
            IUnitOfWork unitOfWork,
            IRepository<TransportMaster> transportRepository,
            IMapper mapper,
            ILogger<TransportService> logger)
        {
            _unitOfWork = unitOfWork;
            _transportRepository = transportRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TransportResponseDto>> GetAllTransportsAsync()
        {
            _logger.LogDebug("Getting all transports");
            var transports = await _transportRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {TransportCount} transports", transports.Count());
            return _mapper.Map<IEnumerable<TransportResponseDto>>(transports);
        }

        public async Task<TransportResponseDto?> GetTransportByIdAsync(int id)
        {
            _logger.LogDebug("Getting transport by ID: {TransportId}", id);
            var transport = await _transportRepository.GetByIdAsync(id);
            if (transport == null)
            {
                _logger.LogWarning("Transport {TransportId} not found or inactive", id);
                return null;
            }
            return _mapper.Map<TransportResponseDto>(transport);
        }

        public async Task<TransportResponseDto> CreateTransportAsync(CreateTransportRequestDto createTransportDto)
        {
            _logger.LogDebug("Creating new transport: {TransportName}", createTransportDto.TransportName);

            // Check if transport is unique
            var existingTransport = await _transportRepository.FirstOrDefaultAsync(m => m.TransportName == createTransportDto.TransportName);
            if (existingTransport != null)
            {
                throw new InvalidOperationException($"Transport with name '{createTransportDto.TransportName}' already exists");
            }

            var transport = _mapper.Map<TransportMaster>(createTransportDto);
            await _transportRepository.AddAsync(transport);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created transport {TransportId} with name: {TransportName}", transport.Id, transport.TransportName);
            return _mapper.Map<TransportResponseDto>(transport);
        }

        public async Task<TransportResponseDto?> UpdateTransportAsync(int id, UpdateTransportRequestDto updateTransportDto)
        {
            _logger.LogDebug("Updating transport {TransportId}", id);

            var transport = await _transportRepository.GetByIdAsync(id);
            if (transport == null)
            {
                _logger.LogWarning("Transport {TransportId} not found for update", id);
                return null;
            }

            // Check if transport is unique (excluding current transport)
            var existingTransport = await _transportRepository.FirstOrDefaultAsync(m => 
                m.TransportName == updateTransportDto.TransportName && m.Id != id);
            if (existingTransport != null)
            {
                throw new InvalidOperationException($"Transport with name '{updateTransportDto.TransportName}' already exists");
            }

            _mapper.Map(updateTransportDto, transport);
            transport.UpdatedAt = DateTime.Now;

            _transportRepository.Update(transport);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated transport {TransportId}", id);
            return _mapper.Map<TransportResponseDto>(transport);
        }

        public async Task<bool> DeleteTransportAsync(int id)
        {
            _logger.LogDebug("Deleting transport {TransportId}", id);

            var transport = await _transportRepository.GetByIdAsync(id);
            if (transport == null)
            {
                _logger.LogWarning("Transport {TransportId} not found for deletion", id);
                return false;
            }
            _transportRepository.Remove(transport);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TransportResponseDto>> SearchTransportsAsync(TransportSearchRequestDto searchDto)
        {
            _logger.LogDebug("Searching transports with name: {TransportName}", searchDto.TransportName);

            var transports = await _transportRepository.FindAsync(m =>
                m.IsActive &&
                (string.IsNullOrEmpty(searchDto.TransportName) || m.TransportName.Contains(searchDto.TransportName)) &&
                (!searchDto.IsActive.HasValue || m.IsActive == searchDto.IsActive.Value));

            return _mapper.Map<IEnumerable<TransportResponseDto>>(transports);
        }

        public async Task<bool> IsTransportUniqueAsync(string transportName, int? excludeId = null)
        {
            var query = await _transportRepository.FindAsync(m => m.TransportName == transportName);
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }
            return !query.Any();
        }
    }
}