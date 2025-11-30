using AutoMapper;
using AvyyanBackend.DTOs.TapeColor;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Services
{
    public class TapeColorService : ITapeColorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TapeColorService> _logger;

        public TapeColorService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TapeColorService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TapeColorResponseDto>> GetAllTapeColorsAsync()
        {
            _logger.LogDebug("Getting all tape colors");
            var tapeColors = await _unitOfWork.TapeColors.GetAllAsync();
            _logger.LogInformation("Retrieved {TapeColorCount} tape colors", tapeColors.Count());
            return _mapper.Map<IEnumerable<TapeColorResponseDto>>(tapeColors);
        }

        public async Task<TapeColorResponseDto?> GetTapeColorByIdAsync(int id)
        {
            _logger.LogDebug("Getting tape color by ID: {TapeColorId}", id);
            var tapeColor = await _unitOfWork.TapeColors.GetByIdAsync(id);
            if (tapeColor == null)
            {
                _logger.LogWarning("Tape color {TapeColorId} not found or inactive", id);
                return null;
            }
            return _mapper.Map<TapeColorResponseDto>(tapeColor);
        }

        public async Task<TapeColorResponseDto> CreateTapeColorAsync(CreateTapeColorRequestDto createTapeColorDto)
        {
            _logger.LogDebug("Creating new tape color: {TapeColor}", createTapeColorDto.TapeColor);

            // Check if tape color is unique
            var existingTapeColor = await _unitOfWork.TapeColors.FirstOrDefaultAsync(m => m.TapeColor == createTapeColorDto.TapeColor);
            if (existingTapeColor != null)
            {
                throw new InvalidOperationException($"Tape color '{createTapeColorDto.TapeColor}' already exists");
            }

            var tapeColor = _mapper.Map<TapeColorMaster>(createTapeColorDto);
            await _unitOfWork.TapeColors.AddAsync(tapeColor);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created tape color {TapeColorId} with name: {TapeColor}", tapeColor.Id, tapeColor.TapeColor);
            return _mapper.Map<TapeColorResponseDto>(tapeColor);
        }

        public async Task<TapeColorResponseDto?> UpdateTapeColorAsync(int id, UpdateTapeColorRequestDto updateTapeColorDto)
        {
            _logger.LogDebug("Updating tape color {TapeColorId}", id);

            var tapeColor = await _unitOfWork.TapeColors.GetByIdAsync(id);
            if (tapeColor == null)
            {
                _logger.LogWarning("Tape color {TapeColorId} not found for update", id);
                return null;
            }

            // Check if tape color is unique (excluding current tape color)
            var existingTapeColor = await _unitOfWork.TapeColors.FirstOrDefaultAsync(m => 
                m.TapeColor == updateTapeColorDto.TapeColor && m.Id != id);
            if (existingTapeColor != null)
            {
                throw new InvalidOperationException($"Tape color '{updateTapeColorDto.TapeColor}' already exists");
            }

            _mapper.Map(updateTapeColorDto, tapeColor);
            tapeColor.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TapeColors.Update(tapeColor);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated tape color {TapeColorId}", id);
            return _mapper.Map<TapeColorResponseDto>(tapeColor);
        }

        public async Task<bool> DeleteTapeColorAsync(int id)
        {
            _logger.LogDebug("Deleting tape color {TapeColorId}", id);

            var tapeColor = await _unitOfWork.TapeColors.GetByIdAsync(id);
            if (tapeColor == null)
            {
                _logger.LogWarning("Tape color {TapeColorId} not found for deletion", id);
                return false;
            }
            _unitOfWork.TapeColors.Remove(tapeColor);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TapeColorResponseDto>> SearchTapeColorsAsync(TapeColorSearchRequestDto searchDto)
        {
            _logger.LogDebug("Searching tape colors with tape color: {TapeColor}", 
                searchDto.TapeColor);

            var tapeColors = await _unitOfWork.TapeColors.FindAsync(m =>
                m.IsActive &&
                (string.IsNullOrEmpty(searchDto.TapeColor) || m.TapeColor.Contains(searchDto.TapeColor)) &&
                (!searchDto.IsActive.HasValue || m.IsActive == searchDto.IsActive.Value));

            return _mapper.Map<IEnumerable<TapeColorResponseDto>>(tapeColors);
        }

        public async Task<bool> IsTapeColorUniqueAsync(string tapeColor, int? excludeId = null)
        {
            var query = await _unitOfWork.TapeColors.FindAsync(m => m.TapeColor == tapeColor);
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }
            return !query.Any();
        }

        // New method to check if tape color is already assigned to a lotment
        public async Task<bool> IsTapeColorAssignedToLotmentAsync(string tapeColor, string lotmentId)
        {
            _logger.LogDebug("Checking if tape color {TapeColor} is assigned to lotment {LotmentId}", tapeColor, lotmentId);

            // Check in ProductionAllotment table for the same tape color but different lotment
            var productionAllotmentWithSameTape = await _unitOfWork.ProductionAllotments
                .FindAsync(pa => pa.TapeColor == tapeColor && pa.AllotmentId != lotmentId);
            
            if (productionAllotmentWithSameTape.Any())
            {
                return true;
            }

            // Check in StorageCapture table for the same tape color
            var storageCaptureWithSameTape = await _unitOfWork.StorageCaptures
                .FindAsync(sc => sc.Tape == tapeColor);
            
            return storageCaptureWithSameTape.Any();
        }
    }
}