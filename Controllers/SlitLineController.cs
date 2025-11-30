using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.DTOs.SlitLine;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SlitLineController : ControllerBase
    {
        private readonly ISlitLineService _slitLineService;
        private readonly ILogger<SlitLineController> _logger;

        public SlitLineController(ISlitLineService slitLineService, ILogger<SlitLineController> logger)
        {
            _slitLineService = slitLineService;
            _logger = logger;
        }

        /// <summary>
        /// Get all slit lines
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SlitLineResponseDto>>> GetSlitLines()
        {
            try
            {
                var slitLines = await _slitLineService.GetAllSlitLinesAsync();
                return Ok(slitLines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting slit lines");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get slit line by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SlitLineResponseDto>> GetSlitLine(int id)
        {
            try
            {
                var slitLine = await _slitLineService.GetSlitLineByIdAsync(id);
                if (slitLine == null)
                {
                    return NotFound($"Slit line with ID {id} not found");
                }
                return Ok(slitLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting slit line {SlitLineId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Search slit lines by various criteria
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SlitLineResponseDto>>> SearchSlitLines(
            [FromQuery] string? slitLine,
            [FromQuery] char? slitLineCode,
            [FromQuery] bool? isActive)
        {
            try
            {
                var searchDto = new SlitLineSearchRequestDto
                {
                    SlitLine = slitLine,
                    SlitLineCode = slitLineCode,
                    IsActive = isActive
                };
                var slitLines = await _slitLineService.SearchSlitLinesAsync(searchDto);
                return Ok(slitLines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching slit lines");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new slit line
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SlitLineResponseDto>> CreateSlitLine(CreateSlitLineRequestDto createSlitLineDto)
        {
            try
            {
                var slitLine = await _slitLineService.CreateSlitLineAsync(createSlitLineDto);
                return CreatedAtAction(nameof(GetSlitLine), new { id = slitLine.Id }, slitLine);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating slit line");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update a slit line
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<SlitLineResponseDto>> UpdateSlitLine(int id, UpdateSlitLineRequestDto updateSlitLineDto)
        {
            try
            {
                var slitLine = await _slitLineService.UpdateSlitLineAsync(id, updateSlitLineDto);
                if (slitLine == null)
                {
                    return NotFound($"Slit line with ID {id} not found");
                }
                return Ok(slitLine);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating slit line {SlitLineId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a slit line (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSlitLine(int id)
        {
            try
            {
                var result = await _slitLineService.DeleteSlitLineAsync(id);
                if (!result)
                {
                    return NotFound($"Slit line with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting slit line {SlitLineId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}