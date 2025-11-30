using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.DTOs.Courier;
using AvyyanBackend.Interfaces;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourierController : ControllerBase
    {
        private readonly ICourierService _courierService;
        private readonly ILogger<CourierController> _logger;

        public CourierController(ICourierService courierService, ILogger<CourierController> logger)
        {
            _courierService = courierService;
            _logger = logger;
        }

        /// <summary>
        /// Get all couriers
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourierResponseDto>>> GetCouriers()
        {
            try
            {
                var couriers = await _courierService.GetAllCouriersAsync();
                return Ok(couriers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting couriers");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get courier by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CourierResponseDto>> GetCourier(int id)
        {
            try
            {
                var courier = await _courierService.GetCourierByIdAsync(id);
                if (courier == null)
                {
                    return NotFound($"Courier with ID {id} not found");
                }
                return Ok(courier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting courier {CourierId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Search couriers by name
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CourierResponseDto>>> SearchCouriers(
            [FromQuery] string? courierName,
            [FromQuery] bool? isActive)
        {
            try
            {
                var searchDto = new CourierSearchRequestDto
                {
                    CourierName = courierName,
                    IsActive = isActive
                };
                var couriers = await _courierService.SearchCouriersAsync(searchDto);
                return Ok(couriers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching couriers");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new courier
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CourierResponseDto>> CreateCourier(CreateCourierRequestDto createCourierDto)
        {
            try
            {
                var courier = await _courierService.CreateCourierAsync(createCourierDto);
                return CreatedAtAction(nameof(GetCourier), new { id = courier.Id }, courier);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating courier");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update a courier
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CourierResponseDto>> UpdateCourier(int id, UpdateCourierRequestDto updateCourierDto)
        {
            try
            {
                var courier = await _courierService.UpdateCourierAsync(id, updateCourierDto);
                if (courier == null)
                {
                    return NotFound($"Courier with ID {id} not found");
                }
                return Ok(courier);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating courier {CourierId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a courier (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourier(int id)
        {
            try
            {
                var result = await _courierService.DeleteCourierAsync(id);
                if (!result)
                {
                    return NotFound($"Courier with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting courier {CourierId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}