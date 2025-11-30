using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.DTOs.Shift;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;
        private readonly ILogger<ShiftController> _logger;

        public ShiftController(IShiftService shiftService, ILogger<ShiftController> logger)
        {
            _shiftService = shiftService;
            _logger = logger;
        }

        /// <summary>
        /// Get all shifts
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftResponseDto>>> GetShifts()
        {
            try
            {
                var shifts = await _shiftService.GetAllShiftsAsync();
                return Ok(shifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting shifts");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get shift by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftResponseDto>> GetShift(int id)
        {
            try
            {
                var shift = await _shiftService.GetShiftByIdAsync(id);
                if (shift == null)
                {
                    return NotFound($"Shift with ID {id} not found");
                }
                return Ok(shift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting shift {ShiftId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Search shifts by various criteria
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ShiftResponseDto>>> SearchShifts(
            [FromQuery] string? shiftName,
            [FromQuery] bool? isActive)
        {
            try
            {
                var searchDto = new ShiftSearchRequestDto
                {
                    ShiftName = shiftName,
                    IsActive = isActive
                };
                var shifts = await _shiftService.SearchShiftsAsync(searchDto);
                return Ok(shifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching shifts");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new shift
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ShiftResponseDto>> CreateShift(CreateShiftRequestDto createShiftDto)
        {
            try
            {
                var shift = await _shiftService.CreateShiftAsync(createShiftDto);
                return CreatedAtAction(nameof(GetShift), new { id = shift.Id }, shift);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating shift");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update a shift
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ShiftResponseDto>> UpdateShift(int id, UpdateShiftRequestDto updateShiftDto)
        {
            try
            {
                var shift = await _shiftService.UpdateShiftAsync(id, updateShiftDto);
                if (shift == null)
                {
                    return NotFound($"Shift with ID {id} not found");
                }
                return Ok(shift);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating shift {ShiftId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a shift (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShift(int id)
        {
            try
            {
                var result = await _shiftService.DeleteShiftAsync(id);
                if (!result)
                {
                    return NotFound($"Shift with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting shift {ShiftId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}