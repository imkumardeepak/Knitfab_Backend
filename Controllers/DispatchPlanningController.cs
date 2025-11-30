using AvyyanBackend.DTOs.DispatchPlanning;
using AvyyanBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispatchPlanningController : ControllerBase
    {
        private readonly DispatchPlanningService _service;

        public DispatchPlanningController(DispatchPlanningService service)
        {
            _service = service;
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

        [HttpPost]
        public async Task<ActionResult<DispatchPlanningDto>> Create(CreateDispatchPlanningDto createDto)
        {
            try
            {
                var dispatchPlanning = await _service.CreateAsync(createDto);
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
                var dispatchPlanning = await _service.UpdateAsync(id, updateDto);
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
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();

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
                return CreatedAtAction(nameof(GetDispatchedRolls), new { id = dispatchedRoll.Id }, dispatchedRoll);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        // New endpoint to update dispatch status based on roll counts
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

        // New endpoint to create multiple dispatch planning records with the same dispatch order ID
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<DispatchPlanningDto>>> CreateBatch(
      [FromBody] IEnumerable<CreateDispatchPlanningRequestDto> createDtos)
        {
            try
            {
                var results = await _service.CreateBatchAsync(createDtos);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // New endpoint to get dispatched rolls ordered by lotNo and fgRoll sequence
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
    }
}