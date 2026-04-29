using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.Filters;
using AvyyanBackend.DTOs.FabricStructure;
using AvyyanBackend.Interfaces;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FabricStructureController : ControllerBase
    {
        private readonly IFabricStructureService _fabricStructureService;
        private readonly ILogger<FabricStructureController> _logger;
        private readonly IAuditLogService _auditLogService;

        public FabricStructureController(IFabricStructureService fabricStructureService, ILogger<FabricStructureController> logger, IAuditLogService auditLogService)
        {
            _fabricStructureService = fabricStructureService;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        /// <summary>Get all fabric structures</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FabricStructureResponseDto>>> GetFabricStructures()
        {
            try
            {
                var fabricStructures = await _fabricStructureService.GetAllFabricStructuresAsync();
                return Ok(fabricStructures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting fabric structures");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Get fabric structure by ID</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FabricStructureResponseDto>> GetFabricStructure(int id)
        {
            try
            {
                var fabricStructure = await _fabricStructureService.GetFabricStructureByIdAsync(id);
                if (fabricStructure == null)
                    return NotFound($"Fabric structure with ID {id} not found");
                return Ok(fabricStructure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting fabric structure {FabricStructureId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Search fabric structures by name</summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FabricStructureResponseDto>>> SearchFabricStructures(
            [FromQuery] string? fabricstr,
            [FromQuery] bool? isActive)
        {
            try
            {
                var searchDto = new FabricStructureSearchRequestDto { Fabricstr = fabricstr, IsActive = isActive };
                var fabricStructures = await _fabricStructureService.SearchFabricStructuresAsync(searchDto);
                return Ok(fabricStructures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching fabric structures");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Create a new fabric structure</summary>
        [HttpPost]
        public async Task<ActionResult<FabricStructureResponseDto>> CreateFabricStructure(CreateFabricStructureRequestDto createFabricStructureDto)
        {
            try
            {
                var fabricStructure = await _fabricStructureService.CreateFabricStructureAsync(createFabricStructureDto);

                await _auditLogService.LogAsync(
                    action: "CREATE",
                    module: "FabricStructure",
                    entityId: fabricStructure.Id,
                    entityName: fabricStructure.Fabricstr,
                    changeSummary: $"Created Fabric Structure '{fabricStructure.Fabricstr}' — Std Efficiency: {fabricStructure.Standardeffencny}",
                    newValues: new { fabricStructure.Fabricstr, fabricStructure.Standardeffencny }
                );

                return CreatedAtAction(nameof(GetFabricStructure), new { id = fabricStructure.Id }, fabricStructure);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating fabric structure");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Update a fabric structure</summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<FabricStructureResponseDto>> UpdateFabricStructure(int id, UpdateFabricStructureRequestDto updateFabricStructureDto)
        {
            try
            {
                var fabricStructure = await _fabricStructureService.UpdateFabricStructureAsync(id, updateFabricStructureDto);
                if (fabricStructure == null)
                    return NotFound($"Fabric structure with ID {id} not found");

                await _auditLogService.LogAsync(
                    action: "UPDATE",
                    module: "FabricStructure",
                    entityId: id,
                    entityName: fabricStructure.Fabricstr,
                    changeSummary: $"Updated Fabric Structure '{fabricStructure.Fabricstr}' — Std Efficiency: {fabricStructure.Standardeffencny}",
                    newValues: new { fabricStructure.Fabricstr, fabricStructure.Standardeffencny, fabricStructure.IsActive }
                );

                return Ok(fabricStructure);
            }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating fabric structure {FabricStructureId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>Delete a fabric structure (soft delete)</summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFabricStructure(int id)
        {
            try
            {
                var result = await _fabricStructureService.DeleteFabricStructureAsync(id);
                if (!result)
                    return NotFound($"Fabric structure with ID {id} not found");

                await _auditLogService.LogAsync(
                    action: "DELETE",
                    module: "FabricStructure",
                    entityId: id,
                    changeSummary: $"Deleted Fabric Structure #{id}"
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting fabric structure {FabricStructureId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
