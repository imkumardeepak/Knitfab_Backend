using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.DTOs.TapeColor;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TapeColorController : ControllerBase
    {
        private readonly ITapeColorService _tapeColorService;
        private readonly ILogger<TapeColorController> _logger;

        public TapeColorController(ITapeColorService tapeColorService, ILogger<TapeColorController> logger)
        {
            _tapeColorService = tapeColorService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tape colors
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TapeColorResponseDto>>> GetTapeColors()
        {
            try
            {
                var tapeColors = await _tapeColorService.GetAllTapeColorsAsync();
                return Ok(tapeColors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tape colors");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get tape color by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TapeColorResponseDto>> GetTapeColor(int id)
        {
            try
            {
                var tapeColor = await _tapeColorService.GetTapeColorByIdAsync(id);
                if (tapeColor == null)
                {
                    return NotFound($"Tape color with ID {id} not found");
                }
                return Ok(tapeColor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tape color {TapeColorId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Search tape colors by various criteria
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TapeColorResponseDto>>> SearchTapeColors(
            [FromQuery] string? tapeColor,
            [FromQuery] bool? isActive)
        {
            try
            {
                var searchDto = new TapeColorSearchRequestDto
                {
                    TapeColor = tapeColor,
                    IsActive = isActive
                };
                var tapeColors = await _tapeColorService.SearchTapeColorsAsync(searchDto);
                return Ok(tapeColors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching tape colors");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Check if a tape color is already assigned to a lotment
        /// </summary>
        [HttpGet("is-assigned/{lotmentId}")]
        public async Task<ActionResult<bool>> IsTapeColorAssignedToLotment(
            string lotmentId,
            [FromQuery] string tapeColor)
        {
            try
            {
                var isAssigned = await _tapeColorService.IsTapeColorAssignedToLotmentAsync(tapeColor, lotmentId);
                return Ok(isAssigned);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if tape color {TapeColor} is assigned to lotment {LotmentId}", tapeColor, lotmentId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new tape color
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TapeColorResponseDto>> CreateTapeColor(CreateTapeColorRequestDto createTapeColorDto)
        {
            try
            {
                var tapeColor = await _tapeColorService.CreateTapeColorAsync(createTapeColorDto);
                return CreatedAtAction(nameof(GetTapeColor), new { id = tapeColor.Id }, tapeColor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating tape color");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update a tape color
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TapeColorResponseDto>> UpdateTapeColor(int id, UpdateTapeColorRequestDto updateTapeColorDto)
        {
            try
            {
                var tapeColor = await _tapeColorService.UpdateTapeColorAsync(id, updateTapeColorDto);
                if (tapeColor == null)
                {
                    return NotFound($"Tape color with ID {id} not found");
                }
                return Ok(tapeColor);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating tape color {TapeColorId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a tape color (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTapeColor(int id)
        {
            try
            {
                var result = await _tapeColorService.DeleteTapeColorAsync(id);
                if (!result)
                {
                    return NotFound($"Tape color with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting tape color {TapeColorId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}