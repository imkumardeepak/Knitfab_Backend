using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.DTOs.Transport;
using AvyyanBackend.Interfaces;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransportController : ControllerBase
    {
        private readonly ITransportService _transportService;
        private readonly ILogger<TransportController> _logger;

        public TransportController(ITransportService transportService, ILogger<TransportController> logger)
        {
            _transportService = transportService;
            _logger = logger;
        }

        /// <summary>
        /// Get all transports
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransportResponseDto>>> GetTransports()
        {
            try
            {
                var transports = await _transportService.GetAllTransportsAsync();
                return Ok(transports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transports");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get transport by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransportResponseDto>> GetTransport(int id)
        {
            try
            {
                var transport = await _transportService.GetTransportByIdAsync(id);
                if (transport == null)
                {
                    return NotFound($"Transport with ID {id} not found");
                }
                return Ok(transport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting transport {TransportId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Search transports by name
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TransportResponseDto>>> SearchTransports(
            [FromQuery] string? transportName,
            [FromQuery] bool? isActive)
        {
            try
            {
                var searchDto = new TransportSearchRequestDto
                {
                    TransportName = transportName,
                    IsActive = isActive
                };
                var transports = await _transportService.SearchTransportsAsync(searchDto);
                return Ok(transports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching transports");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new transport
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransportResponseDto>> CreateTransport(CreateTransportRequestDto createTransportDto)
        {
            try
            {
                var transport = await _transportService.CreateTransportAsync(createTransportDto);
                return CreatedAtAction(nameof(GetTransport), new { id = transport.Id }, transport);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating transport");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update a transport
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TransportResponseDto>> UpdateTransport(int id, UpdateTransportRequestDto updateTransportDto)
        {
            try
            {
                var transport = await _transportService.UpdateTransportAsync(id, updateTransportDto);
                if (transport == null)
                {
                    return NotFound($"Transport with ID {id} not found");
                }
                return Ok(transport);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating transport {TransportId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a transport (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransport(int id)
        {
            try
            {
                var result = await _transportService.DeleteTransportAsync(id);
                if (!result)
                {
                    return NotFound($"Transport with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting transport {TransportId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}