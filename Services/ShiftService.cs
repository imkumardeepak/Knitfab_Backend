using AutoMapper;
using AvyyanBackend.DTOs.Shift;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ShiftService> _logger;

        public ShiftService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ShiftService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ShiftResponseDto>> GetAllShiftsAsync()
        {
            _logger.LogDebug("Getting all shifts");
            var shifts = await _unitOfWork.Shifts.GetAllAsync();
            _logger.LogInformation("Retrieved {ShiftCount} shifts", shifts.Count());
            return _mapper.Map<IEnumerable<ShiftResponseDto>>(shifts);
        }

        public async Task<ShiftResponseDto?> GetShiftByIdAsync(int id)
        {
            _logger.LogDebug("Getting shift by ID: {ShiftId}", id);
            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            if (shift == null)
            {
                _logger.LogWarning("Shift {ShiftId} not found or inactive", id);
                return null;
            }
            return _mapper.Map<ShiftResponseDto>(shift);
        }

        public async Task<ShiftResponseDto> CreateShiftAsync(CreateShiftRequestDto createShiftDto)
        {
            _logger.LogDebug("Creating new shift: {ShiftName}", createShiftDto.ShiftName);

            // Check if shift name is unique
            var existingShift = await _unitOfWork.Shifts.FirstOrDefaultAsync(m => m.ShiftName == createShiftDto.ShiftName);
            if (existingShift != null)
            {
                throw new InvalidOperationException($"Shift with name '{createShiftDto.ShiftName}' already exists");
            }

            var shift = _mapper.Map<ShiftMaster>(createShiftDto);
            await _unitOfWork.Shifts.AddAsync(shift);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created shift {ShiftId} with name: {ShiftName}", shift.Id, shift.ShiftName);
            return _mapper.Map<ShiftResponseDto>(shift);
        }

        public async Task<ShiftResponseDto?> UpdateShiftAsync(int id, UpdateShiftRequestDto updateShiftDto)
        {
            _logger.LogDebug("Updating shift {ShiftId}", id);

            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            if (shift == null)
            {
                _logger.LogWarning("Shift {ShiftId} not found for update", id);
                return null;
            }

            // Check if shift name is unique (excluding current shift)
            var existingShift = await _unitOfWork.Shifts.FirstOrDefaultAsync(m => 
                m.ShiftName == updateShiftDto.ShiftName && m.Id != id);
            if (existingShift != null)
            {
                throw new InvalidOperationException($"Shift with name '{updateShiftDto.ShiftName}' already exists");
            }

            _mapper.Map(updateShiftDto, shift);
            shift.UpdatedAt = DateTime.Now;

            _unitOfWork.Shifts.Update(shift);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated shift {ShiftId}", id);
            return _mapper.Map<ShiftResponseDto>(shift);
        }

        public async Task<bool> DeleteShiftAsync(int id)
        {
            _logger.LogDebug("Deleting shift {ShiftId}", id);

            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            if (shift == null)
            {
                _logger.LogWarning("Shift {ShiftId} not found for deletion", id);
                return false;
            }
            _unitOfWork.Shifts.Remove(shift);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ShiftResponseDto>> SearchShiftsAsync(ShiftSearchRequestDto searchDto)
        {
            _logger.LogDebug("Searching shifts with shift name: {ShiftName}", 
                searchDto.ShiftName);

            var shifts = await _unitOfWork.Shifts.FindAsync(m =>
                m.IsActive &&
                (string.IsNullOrEmpty(searchDto.ShiftName) || m.ShiftName.Contains(searchDto.ShiftName)) &&
                (!searchDto.IsActive.HasValue || m.IsActive == searchDto.IsActive.Value));

            return _mapper.Map<IEnumerable<ShiftResponseDto>>(shifts);
        }

        public async Task<bool> IsShiftNameUniqueAsync(string shiftName, int? excludeId = null)
        {
            var query = await _unitOfWork.Shifts.FindAsync(m => m.ShiftName == shiftName);
            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }
            return !query.Any();
        }
    }
}