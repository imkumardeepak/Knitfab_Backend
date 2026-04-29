using AvyyanBackend.DTOs.DispatchPlanning;
using AvyyanBackend.Services;
using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.Filters;
using AvyyanBackend.Interfaces;
using Npgsql;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispatchPlanningController : ControllerBase
    {
        private readonly DispatchPlanningService _service;
        private readonly IAuditLogService _auditLogService;

        public DispatchPlanningController(DispatchPlanningService service, IAuditLogService auditLogService)
        {
            _service = service;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DispatchPlanningDto>>> GetAll()
        {
            var dispatchPlannings = await _service.GetAllAsync();
            return Ok(dispatchPlannings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DispatchPlanningDto>> GetById(int id)
        {
            var dispatchPlanning = await _service.GetByIdAsync(id);
            if (dispatchPlanning == null)
                return NotFound();

            return Ok(dispatchPlanning);
        }

        // GET api/DispatchPlanning/by-dispatch-order/{dispatchOrderId}
        [HttpGet("by-dispatch-order/{dispatchOrderId}")]
        public async Task<ActionResult<IEnumerable<DispatchPlanningDto>>> GetByDispatchOrderId(string dispatchOrderId)
        {
            try
            {
                var dispatchPlannings = await _service.GetByDispatchOrderIdAsync(dispatchOrderId);
                return Ok(dispatchPlannings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/DispatchPlanning/fully-dispatched-orders
        [HttpGet("fully-dispatched-orders")]
        public async Task<ActionResult<IEnumerable<object>>> GetFullyDispatchedOrders()
        {
            try
            {
                var orders = await _service.GetFullyDispatchedOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<DispatchPlanningDto>> Create(CreateDispatchPlanningDto createDto)
        {
            try
            {
                var dispatchPlanning = await _service.CreateAsync(createDto);

                await _auditLogService.LogAsync(
                    action: "CREATE",
                    module: "Dispatch",
                    entityId: dispatchPlanning.Id,
                    entityName: dispatchPlanning.DispatchOrderId,
                    changeSummary: $"Created Dispatch Order {dispatchPlanning.DispatchOrderId} — Lot: {dispatchPlanning.LotNo}, Customer: {dispatchPlanning.CustomerName}",
                    newValues: new { dispatchPlanning.DispatchOrderId, dispatchPlanning.LotNo, dispatchPlanning.CustomerName, dispatchPlanning.VehicleNo }
                );

                return CreatedAtAction(nameof(GetById), new { id = dispatchPlanning.Id }, dispatchPlanning);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DispatchPlanningDto>> Update(int id, UpdateDispatchPlanningDto updateDto)
        {
            try
            {
                var old = await _service.GetByIdAsync(id);
                var dispatchPlanning = await _service.UpdateAsync(id, updateDto);

                await _auditLogService.LogAsync(
                    action: "UPDATE",
                    module: "Dispatch",
                    entityId: id,
                    entityName: dispatchPlanning.DispatchOrderId,
                    changeSummary: $"Updated Dispatch Order {dispatchPlanning.DispatchOrderId}",
                    oldValues: old == null ? null : new { old.VehicleNo, old.IsFullyDispatched, old.CustomerName },
                    newValues: new { dispatchPlanning.VehicleNo, dispatchPlanning.IsFullyDispatched, dispatchPlanning.CustomerName }
                );

                return Ok(dispatchPlanning);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();

            await _auditLogService.LogAsync(
                action: "DELETE",
                module: "Dispatch",
                entityId: id,
                entityName: existing?.DispatchOrderId,
                changeSummary: $"Deleted Dispatch Planning record #{id} from Order {existing?.DispatchOrderId}",
                oldValues: existing == null ? null : new { existing.DispatchOrderId, existing.LotNo, existing.CustomerName, existing.VehicleNo }
            );

            return NoContent();
        }

        [HttpGet("{id}/dispatched-rolls")]
        public async Task<ActionResult<IEnumerable<DispatchedRollDto>>> GetDispatchedRolls(int id)
        {
            var dispatchedRolls = await _service.GetDispatchedRollsByPlanningIdAsync(id);
            return Ok(dispatchedRolls);
        }

        [HttpPost("dispatched-rolls")]
        public async Task<ActionResult<DispatchedRollDto>> CreateDispatchedRoll(DispatchedRollDto dto)
        {
            try
            {
                var dispatchedRoll = await _service.CreateDispatchedRollAsync(dto);

                await _auditLogService.LogAsync(
                    action: "LOAD",
                    module: "Dispatch",
                    entityName: dto.LotNo,
                    changeSummary: $"Loaded Roll {dto.FGRollNo} / Lot {dto.LotNo} into Dispatch Plan #{dto.DispatchPlanningId}"
                );

                return CreatedAtAction(nameof(GetDispatchedRolls), new { id = dispatchedRoll.Id }, dispatchedRoll);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating dispatched roll: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while creating the dispatched roll." });
            }
        }

        [HttpPost("dispatched-rolls/bulk")]
        public async Task<ActionResult<IEnumerable<DispatchedRollDto>>> CreateDispatchedRollsBulk([FromBody] IEnumerable<DispatchedRollDto> dtos)
        {
            try
            {
                var dispatchedRolls = await _service.CreateDispatchedRollsBulkAsync(dtos);
                var dtoList = dtos.ToList();

                await _auditLogService.LogAsync(
                    action: "LOAD",
                    module: "Dispatch",
                    entityName: dtoList.FirstOrDefault()?.LotNo,
                    changeSummary: $"Bulk loaded {dtoList.Count} rolls into Dispatch Plan #{dtoList.FirstOrDefault()?.DispatchPlanningId}"
                );

                return Ok(dispatchedRolls);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Update dispatch status based on roll counts
        [HttpPut("{id}/status")]
        public async Task<ActionResult<DispatchPlanningDto>> UpdateDispatchStatus(int id, [FromBody] decimal totalDispatchedRolls)
        {
            try
            {
                var dispatchPlanning = await _service.UpdateDispatchStatusAsync(id, totalDispatchedRolls);
                return Ok(dispatchPlanning);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Create multiple dispatch planning records with the same dispatch order ID
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<DispatchPlanningDto>>> CreateBatch(
      [FromBody] IEnumerable<CreateDispatchPlanningRequestDto> createDtos)
        {
            try
            {
                var results = await _service.CreateBatchAsync(createDtos);

                var dtoList = createDtos.ToList();
                await _auditLogService.LogAsync(
                    action: "CREATE",
                    module: "Dispatch",
                    entityName: dtoList.FirstOrDefault()?.LotNo,
                    changeSummary: $"Created batch of {dtoList.Count} Dispatch Planning entries for Lot {dtoList.FirstOrDefault()?.LotNo} / Customer {dtoList.FirstOrDefault()?.CustomerName}"
                );

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Get dispatched rolls ordered by lotNo and fgRoll sequence
        [HttpGet("ordered-dispatched-rolls/{dispatchOrderId}")]
        public async Task<ActionResult<IEnumerable<DispatchedRollDto>>> GetOrderedDispatchedRolls(string dispatchOrderId)
        {
            try
            {
                var dispatchedRolls = await _service.GetOrderedDispatchedRollsByDispatchOrderIdAsync(dispatchOrderId);
                return Ok(dispatchedRolls);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Delete a specific dispatched roll
        [HttpDelete("dispatched-rolls/{id}")]
        public async Task<IActionResult> DeleteDispatchedRoll(int id)
        {
            var result = await _service.DeleteDispatchedRollAsync(id);
            if (!result)
                return NotFound();

            await _auditLogService.LogAsync(
                action: "UNLOAD",
                module: "Dispatch",
                entityId: id,
                changeSummary: $"Removed Dispatched Roll #{id} from dispatch"
            );

            return NoContent();
        }

        [HttpDelete("dispatch-order/{dispatchOrderId}")]
        public async Task<IActionResult> DeleteDispatchOrder(string dispatchOrderId)
        {
            var result = await _service.DeleteByDispatchOrderIdAsync(dispatchOrderId);
            if (!result) return NotFound();

            await _auditLogService.LogAsync(
                action: "CANCEL",
                module: "Dispatch",
                entityName: dispatchOrderId,
                changeSummary: $"Cancelled and deleted entire Dispatch Order {dispatchOrderId}"
            );

            return NoContent();
        }

        [HttpPut("dispatch-order/{dispatchOrderId}/unload")]
        public async Task<IActionResult> UnloadDispatchOrder(string dispatchOrderId)
        {
            var result = await _service.UnloadDispatchOrderAsync(dispatchOrderId);
            if (!result) return NotFound();

            await _auditLogService.LogAsync(
                action: "UNLOAD",
                module: "Dispatch",
                entityName: dispatchOrderId,
                changeSummary: $"Unloaded all rolls from Dispatch Order {dispatchOrderId}"
            );

            return NoContent();
        }
    }
}
