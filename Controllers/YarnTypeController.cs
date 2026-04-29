using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.Filters;
using AvyyanBackend.DTOs.YarnType;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequirePermission("Yarn Type")]
    [Authorize]
    public class YarnTypeController : ControllerBase
    {
        private readonly IYarnTypeService _yarnTypeService;
        private readonly ILogger<YarnTypeController> _logger;
        private readonly IAuditLogService _auditLogService;

        public YarnTypeController(IYarnTypeService yarnTypeService, ILogger<YarnTypeController> logger, IAuditLogService auditLogService)
        {
            _yarnTypeService = yarnTypeService;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        /// <summary>Get all yarn types</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<YarnTypeResponseDto>>> GetAllYarnTypes()
        {
            try
            {
                var yarnTypes = await _yarnTypeService.GetAllYarnTypesAsync();
                return Ok(yarnTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving yarn types");
                return StatusCode(500, "An error occurred while retrieving yarn types");
            }
        }

        /// <summary>Get a specific yarn type by ID</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<YarnTypeResponseDto>> GetYarnType(int id)
        {
            try
            {
                var yarnType = await _yarnTypeService.GetYarnTypeByIdAsync(id);
                if (yarnType == null)
                    return NotFound($"Yarn type with ID {id} not found");
                return Ok(yarnType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving yarn type {YarnTypeId}", id);
                return StatusCode(500, "An error occurred while retrieving the yarn type");
            }
        }

        /// <summary>Create a new yarn type</summary>
        [HttpPost]
        public async Task<ActionResult<YarnTypeResponseDto>> CreateYarnType(CreateYarnTypeRequestDto createYarnTypeDto)
        {
            try
            {
                var yarnType = await _yarnTypeService.CreateYarnTypeAsync(createYarnTypeDto);

                await _auditLogService.LogAsync(
                    action: "CREATE",
                    module: "YarnType",
                    entityId: yarnType.Id,
                    entityName: yarnType.YarnType,
                    changeSummary: $"Created Yarn Type '{yarnType.YarnType}'",
                    newValues: new { yarnType.YarnType }
                );

                return CreatedAtAction(nameof(GetYarnType), new { id = yarnType.Id }, yarnType);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating yarn type");
                return StatusCode(500, "An error occurred while creating the yarn type");
            }
        }

        /// <summary>Update an existing yarn type</summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<YarnTypeResponseDto>> UpdateYarnType(int id, UpdateYarnTypeRequestDto updateYarnTypeDto)
        {
            try
            {
                var yarnType = await _yarnTypeService.UpdateYarnTypeAsync(id, updateYarnTypeDto);
                if (yarnType == null)
                    return NotFound($"Yarn type with ID {id} not found");

                await _auditLogService.LogAsync(
                    action: "UPDATE",
                    module: "YarnType",
                    entityId: id,
                    entityName: yarnType.YarnType,
                    changeSummary: $"Updated Yarn Type '{yarnType.YarnType}'",
                    newValues: new { yarnType.YarnType, yarnType.IsActive }
                );

                return Ok(yarnType);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating yarn type {YarnTypeId}", id);
                return StatusCode(500, "An error occurred while updating the yarn type");
            }
        }

        /// <summary>Delete a yarn type</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteYarnType(int id)
        {
            try
            {
                var result = await _yarnTypeService.DeleteYarnTypeAsync(id);
                if (!result)
                    return NotFound($"Yarn type with ID {id} not found");

                await _auditLogService.LogAsync(
                    action: "DELETE",
                    module: "YarnType",
                    entityId: id,
                    changeSummary: $"Deleted Yarn Type #{id}"
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting yarn type {YarnTypeId}", id);
                return StatusCode(500, "An error occurred while deleting the yarn type");
            }
        }

        /// <summary>Search yarn types</summary>
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<YarnTypeResponseDto>>> SearchYarnTypes(YarnTypeSearchRequestDto searchDto)
        {
            try
            {
                var yarnTypes = await _yarnTypeService.SearchYarnTypesAsync(searchDto);
                return Ok(yarnTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching yarn types");
                return StatusCode(500, "An error occurred while searching yarn types");
            }
        }

        /// <summary>Check if a yarn type is unique</summary>
        [HttpGet("unique")]
        public async Task<ActionResult<bool>> IsYarnTypeUnique([FromQuery] string yarnType, [FromQuery] int? excludeId = null)
        {
            try
            {
                var isUnique = await _yarnTypeService.IsYarnTypeUniqueAsync(yarnType, excludeId);
                return Ok(isUnique);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking yarn type uniqueness");
                return StatusCode(500, "An error occurred while checking yarn type uniqueness");
            }
        }
    }
}
