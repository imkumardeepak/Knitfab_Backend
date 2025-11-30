using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.DTOs.SalesOrder;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalesOrderWebController : ControllerBase
    {
        private readonly ISalesOrderWebService _salesOrderWebService;
        private readonly ILogger<SalesOrderWebController> _logger;

        public SalesOrderWebController(ISalesOrderWebService salesOrderWebService, ILogger<SalesOrderWebController> logger)
        {
            _salesOrderWebService = salesOrderWebService;
            _logger = logger;
        }

        /// <summary>
        /// Get the next serial number for sales order items
        /// </summary>
        [HttpGet("next-serial-number")]
        public async Task<ActionResult<string>> GetNextSerialNumber()
        {
            try
            {
                var serialNumber = await _salesOrderWebService.GetNextSerialNumberAsync();
                return Ok(serialNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting next serial number");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get all sales orders web
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderWebResponseDto>>> GetSalesOrdersWeb()
        {
            try
            {
                var salesOrdersWeb = await _salesOrderWebService.GetAllAsync();
                return Ok(salesOrdersWeb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sales orders web");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get sales order web by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderWebResponseDto>> GetSalesOrderWeb(int id)
        {
            try
            {
                var salesOrderWeb = await _salesOrderWebService.GetByIdAsync(id);
                if (salesOrderWeb == null)
                {
                    return NotFound($"Sales order web with ID {id} not found");
                }
                return Ok(salesOrderWeb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting sales order web {SalesOrderWebId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new sales order web
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SalesOrderWebResponseDto>> CreateSalesOrderWeb(CreateSalesOrderWebRequestDto createSalesOrderWebDto)
        {
            try
            {
                var salesOrderWeb = await _salesOrderWebService.CreateAsync(createSalesOrderWebDto);
                return CreatedAtAction(nameof(GetSalesOrderWeb), new { id = salesOrderWeb.Id }, salesOrderWeb);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating sales order web");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update a sales order web
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<SalesOrderWebResponseDto>> UpdateSalesOrderWeb(int id, UpdateSalesOrderWebRequestDto updateSalesOrderWebDto)
        {
            try
            {
                var salesOrderWeb = await _salesOrderWebService.UpdateAsync(id, updateSalesOrderWebDto);
                if (salesOrderWeb == null)
                {
                    return NotFound($"Sales order web with ID {id} not found");
                }
                return Ok(salesOrderWeb);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating sales order web {SalesOrderWebId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a sales order web
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSalesOrderWeb(int id)
        {
            try
            {
                var result = await _salesOrderWebService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Sales order web with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting sales order web {SalesOrderWebId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Mark a sales order web item as processed
        /// </summary>
        [HttpPut("{salesOrderWebId}/items/{salesOrderItemWebId}/process")]
        public async Task<ActionResult<SalesOrderWebResponseDto>> MarkSalesOrderItemWebAsProcessed(int salesOrderWebId, int salesOrderItemWebId)
        {
            try
            {
                var result = await _salesOrderWebService.MarkSalesOrderItemWebAsProcessedAsync(salesOrderWebId, salesOrderItemWebId);
                if (result == null)
                {
                    return NotFound($"Sales order web item with ID {salesOrderItemWebId} not found in sales order web {salesOrderWebId}");
                }
                
                // Return the updated sales order
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking sales order web item {SalesOrderItemWebId} as processed", salesOrderItemWebId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}