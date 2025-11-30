using AutoMapper;
using AvyyanBackend.DTOs.YarnType;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.Extensions.Logging;

namespace AvyyanBackend.Services
{
    public class YarnTypeService : IYarnTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<YarnTypeMaster> _yarnTypeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<YarnTypeService> _logger;

        public YarnTypeService(
            IUnitOfWork unitOfWork,
            IRepository<YarnTypeMaster> yarnTypeRepository,
            IMapper mapper,
            ILogger<YarnTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _yarnTypeRepository = yarnTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<YarnTypeResponseDto>> GetAllYarnTypesAsync()
        {
            _logger.LogDebug("Getting all yarn types");
            var yarnTypes = await _yarnTypeRepository.GetAllAsync();
            _logger.LogInformation("Retrieved {YarnTypeCount} yarn types", yarnTypes.Count());
            return _mapper.Map<IEnumerable<YarnTypeResponseDto>>(yarnTypes);
        }

        public async Task<YarnTypeResponseDto?> GetYarnTypeByIdAsync(int id)
        {
            _logger.LogDebug("Getting yarn type by ID: {YarnTypeId}", id);
            var yarnType = await _yarnTypeRepository.GetByIdAsync(id);
            if (yarnType == null)
            {
                _logger.LogWarning("Yarn type {YarnTypeId} not found or inactive", id);
                return null;
            }
            return _mapper.Map<YarnTypeResponseDto>(yarnType);
        }

        public async Task<YarnTypeResponseDto> CreateYarnTypeAsync(CreateYarnTypeRequestDto createYarnTypeDto)
        {
            _logger.LogDebug("Creating new yarn type: {YarnTypeName}", createYarnTypeDto.YarnType);

            // Check if yarn type is unique
            var existingYarnType = await _yarnTypeRepository.FirstOrDefaultAsync(y => y.YarnType == createYarnTypeDto.YarnType);
            if (existingYarnType != null)
            {
                throw new InvalidOperationException($"Yarn type '{createYarnTypeDto.YarnType}' already exists");
            }

            var yarnType = _mapper.Map<YarnTypeMaster>(createYarnTypeDto);
            await _yarnTypeRepository.AddAsync(yarnType);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created yarn type {YarnTypeId} with name: {YarnTypeName}", yarnType.Id, yarnType.YarnType);
            return _mapper.Map<YarnTypeResponseDto>(yarnType);
        }

        public async Task<YarnTypeResponseDto?> UpdateYarnTypeAsync(int id, UpdateYarnTypeRequestDto updateYarnTypeDto)
        {
            _logger.LogDebug("Updating yarn type {YarnTypeId}", id);

            var yarnType = await _yarnTypeRepository.GetByIdAsync(id);
            if (yarnType == null)
            {
                _logger.LogWarning("Yarn type {YarnTypeId} not found for update", id);
                return null;
            }

            // Check if yarn type is unique (excluding current yarn type)
            var existingYarnType = await _yarnTypeRepository.FirstOrDefaultAsync(y =>
                y.YarnType == updateYarnTypeDto.YarnType && y.Id != id);
            if (existingYarnType != null)
            {
                throw new InvalidOperationException($"Yarn type '{updateYarnTypeDto.YarnType}' already exists");
            }

            _mapper.Map(updateYarnTypeDto, yarnType);
            yarnType.UpdatedAt = DateTime.UtcNow;

            _yarnTypeRepository.Update(yarnType);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated yarn type {YarnTypeId}", id);
            return _mapper.Map<YarnTypeResponseDto>(yarnType);
        }

        public async Task<bool> DeleteYarnTypeAsync(int id)
        {
            _logger.LogDebug("Deleting yarn type {YarnTypeId}", id);

            var yarnType = await _yarnTypeRepository.GetByIdAsync(id);
            if (yarnType == null)
            {
                _logger.LogWarning("Yarn type {YarnTypeId} not found for deletion", id);
                return false;
            }
            _yarnTypeRepository.Remove(yarnType);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<YarnTypeResponseDto>> SearchYarnTypesAsync(YarnTypeSearchRequestDto searchDto)
        {
            _logger.LogDebug("Searching yarn types with name: {YarnTypeName}", searchDto.YarnType);

            var yarnTypes = await _yarnTypeRepository.FindAsync(y =>
                y.IsActive &&
                (string.IsNullOrEmpty(searchDto.YarnType) || y.YarnType.Contains(searchDto.YarnType)) &&
                (string.IsNullOrEmpty(searchDto.YarnCode) || y.YarnCode.Contains(searchDto.YarnCode ?? string.Empty)) &&
                (string.IsNullOrEmpty(searchDto.ShortCode) || y.ShortCode.Contains(searchDto.ShortCode ?? string.Empty)) &&
                (!searchDto.IsActive.HasValue || y.IsActive == searchDto.IsActive.Value));

            return _mapper.Map<IEnumerable<YarnTypeResponseDto>>(yarnTypes);
        }

        public async Task<bool> IsYarnTypeUniqueAsync(string yarnType, int? excludeId = null)
        {
            var query = await _yarnTypeRepository.FindAsync(y => y.YarnType == yarnType);
            if (excludeId.HasValue)
            {
                query = query.Where(y => y.Id != excludeId.Value);
            }
            return !query.Any();
        }
    }
}