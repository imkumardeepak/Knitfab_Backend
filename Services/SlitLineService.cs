using AvyyanBackend.DTOs.SlitLine;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class SlitLineService : ISlitLineService
    {
        private readonly ISlitLineRepository _slitLineRepository;

        public SlitLineService(ISlitLineRepository slitLineRepository)
        {
            _slitLineRepository = slitLineRepository;
        }

        public async Task<IEnumerable<SlitLineResponseDto>> GetAllSlitLinesAsync()
        {
            var slitLines = await _slitLineRepository.GetAllAsync();
            return slitLines.Select(sl => new SlitLineResponseDto
            {
                Id = sl.Id,
                SlitLine = sl.SlitLine,
                SlitLineCode = sl.SlitLineCode,
                CreatedAt = sl.CreatedAt,
                UpdatedAt = sl.UpdatedAt,
                IsActive = sl.IsActive
            });
        }

        public async Task<SlitLineResponseDto?> GetSlitLineByIdAsync(int id)
        {
            var slitLine = await _slitLineRepository.GetByIdAsync(id);
            if (slitLine == null)
                return null;

            return new SlitLineResponseDto
            {
                Id = slitLine.Id,
                SlitLine = slitLine.SlitLine,
                SlitLineCode = slitLine.SlitLineCode,
                CreatedAt = slitLine.CreatedAt,
                UpdatedAt = slitLine.UpdatedAt,
                IsActive = slitLine.IsActive
            };
        }

        public async Task<SlitLineResponseDto> CreateSlitLineAsync(CreateSlitLineRequestDto createSlitLineDto)
        {
            var slitLine = new SlitLineMaster
            {
                SlitLine = createSlitLineDto.SlitLine,
                SlitLineCode = createSlitLineDto.SlitLineCode
            };

            var createdSlitLine = await _slitLineRepository.CreateAsync(slitLine);

            return new SlitLineResponseDto
            {
                Id = createdSlitLine.Id,
                SlitLine = createdSlitLine.SlitLine,
                SlitLineCode = createdSlitLine.SlitLineCode,
                CreatedAt = createdSlitLine.CreatedAt,
                UpdatedAt = createdSlitLine.UpdatedAt,
                IsActive = createdSlitLine.IsActive
            };
        }

        public async Task<SlitLineResponseDto?> UpdateSlitLineAsync(int id, UpdateSlitLineRequestDto updateSlitLineDto)
        {
            var existingSlitLine = await _slitLineRepository.GetByIdAsync(id);
            if (existingSlitLine == null)
                return null;

            existingSlitLine.SlitLine = updateSlitLineDto.SlitLine;
            existingSlitLine.SlitLineCode = updateSlitLineDto.SlitLineCode;
            existingSlitLine.IsActive = updateSlitLineDto.IsActive;

            var updatedSlitLine = await _slitLineRepository.UpdateAsync(existingSlitLine);
            if (updatedSlitLine == null)
                return null;

            return new SlitLineResponseDto
            {
                Id = updatedSlitLine.Id,
                SlitLine = updatedSlitLine.SlitLine,
                SlitLineCode = updatedSlitLine.SlitLineCode,
                CreatedAt = updatedSlitLine.CreatedAt,
                UpdatedAt = updatedSlitLine.UpdatedAt,
                IsActive = updatedSlitLine.IsActive
            };
        }

        public async Task<bool> DeleteSlitLineAsync(int id)
        {
            return await _slitLineRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<SlitLineResponseDto>> SearchSlitLinesAsync(SlitLineSearchRequestDto searchDto)
        {
            var slitLines = await _slitLineRepository.SearchAsync(
                searchDto.SlitLine,
                searchDto.SlitLineCode,
                searchDto.IsActive
            );

            return slitLines.Select(sl => new SlitLineResponseDto
            {
                Id = sl.Id,
                SlitLine = sl.SlitLine,
                SlitLineCode = sl.SlitLineCode,
                CreatedAt = sl.CreatedAt,
                UpdatedAt = sl.UpdatedAt,
                IsActive = sl.IsActive
            });
        }
    }
}