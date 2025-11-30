using AutoMapper;
using AvyyanBackend.DTOs.FabricStructure;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class FabricStructureService : IFabricStructureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<FabricStructureMaster> _fabricStructureRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FabricStructureService> _logger;

        public FabricStructureService(
            IUnitOfWork unitOfWork,
            IRepository<FabricStructureMaster> fabricStructureRepository,
            IMapper mapper,
            ILogger<FabricStructureService> logger)
        {
            _unitOfWork = unitOfWork;
            _fabricStructureRepository = fabricStructureRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<FabricStructureResponseDto>> GetAllFabricStructuresAsync()
        {
            _logger.LogDebug("Getting all fabric structures");
            var fabricStructures = await _fabricStructureRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {FabricStructureCount} fabric structures", fabricStructures.Count());
            return _mapper.Map<IEnumerable<FabricStructureResponseDto>>(fabricStructures);
        }

        public async Task<FabricStructureResponseDto?> GetFabricStructureByIdAsync(int id)
        {
            _logger.LogDebug("Getting fabric structure by ID: {FabricStructureId}", id);
            var fabricStructure = await _fabricStructureRepository.GetByIdAsync(id);
            if (fabricStructure == null)
            {
                _logger.LogWarning("Fabric structure {FabricStructureId} not found or inactive", id);
                return null;
            }
            return _mapper.Map<FabricStructureResponseDto>(fabricStructure);
        }

        public async Task<FabricStructureResponseDto> CreateFabricStructureAsync(CreateFabricStructureRequestDto createFabricStructureDto)
        {
            _logger.LogDebug("Creating new fabric structure: {FabricStructure}", createFabricStructureDto.Fabricstr);

            // Check if fabric structure is unique
            var existingFabricStructure = await _fabricStructureRepository.FirstOrDefaultAsync(m => m.Fabricstr == createFabricStructureDto.Fabricstr);
            if (existingFabricStructure != null)
            {
                throw new InvalidOperationException($"Fabric structure with name '{createFabricStructureDto.Fabricstr}' already exists");
            }

            var fabricStructure = _mapper.Map<FabricStructureMaster>(createFabricStructureDto);
            await _fabricStructureRepository.AddAsync(fabricStructure);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created fabric structure {FabricStructureId} with name: {FabricStructure}", fabricStructure.Id, fabricStructure.Fabricstr);
            return _mapper.Map<FabricStructureResponseDto>(fabricStructure);
        }

        public async Task<FabricStructureResponseDto?> UpdateFabricStructureAsync(int id, UpdateFabricStructureRequestDto updateFabricStructureDto)
        {
            _logger.LogDebug("Updating fabric structure {FabricStructureId}", id);

            var fabricStructure = await _fabricStructureRepository.GetByIdAsync(id);
            if (fabricStructure == null)
            {
                _logger.LogWarning("Fabric structure {FabricStructureId} not found for update", id);
                return null;
            }

            // Check if fabric structure is unique (excluding current fabric structure)
            var existingFabricStructure = await _fabricStructureRepository.FirstOrDefaultAsync(m => 
                m.Fabricstr == updateFabricStructureDto.Fabricstr && m.Id != id);
            if (existingFabricStructure != null)
            {
                throw new InvalidOperationException($"Fabric structure with name '{updateFabricStructureDto.Fabricstr}' already exists");
            }

            _mapper.Map(updateFabricStructureDto, fabricStructure);
            fabricStructure.UpdatedAt = DateTime.Now;

            _fabricStructureRepository.Update(fabricStructure);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated fabric structure {FabricStructureId}", id);
            return _mapper.Map<FabricStructureResponseDto>(fabricStructure);
        }

        public async Task<bool> DeleteFabricStructureAsync(int id)
        {
            _logger.LogDebug("Deleting fabric structure {FabricStructureId}", id);

            var fabricStructure = await _fabricStructureRepository.GetByIdAsync(id);
            if (fabricStructure == null)
            {
                _logger.LogWarning("Fabric structure {FabricStructureId} not found for deletion", id);
                return false;
            }
            _fabricStructureRepository.Remove(fabricStructure);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<FabricStructureResponseDto>> SearchFabricStructuresAsync(FabricStructureSearchRequestDto searchDto)
        {
            _logger.LogDebug("Searching fabric structures with name: {FabricStructure}", searchDto.Fabricstr);

            var fabricStructures = await _fabricStructureRepository.FindAsync(m =>
                m.IsActive &&
                (string.IsNullOrEmpty(searchDto.Fabricstr) || m.Fabricstr.Contains(searchDto.Fabricstr)) &&
                (!searchDto.IsActive.HasValue || m.IsActive == searchDto.IsActive.Value));

            return _mapper.Map<IEnumerable<FabricStructureResponseDto>>(fabricStructures);
        }

        public async Task<bool> IsFabricStructureUniqueAsync(string fabricStructure, int? excludeId = null)
        {
            var query = await _fabricStructureRepository.FindAsync(m => m.Fabricstr == fabricStructure);
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }
            return !query.Any();
        }
    }
}