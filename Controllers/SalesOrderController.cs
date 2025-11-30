using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.DTOs.SalesOrder;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AvyyanBackend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class SalesOrderController : ControllerBase
	{
		private readonly ISalesOrderService _salesOrderService;
		private readonly ILogger<SalesOrderController> _logger;

		public SalesOrderController(ISalesOrderService salesOrderService, ILogger<SalesOrderController> logger)
		{
			_salesOrderService = salesOrderService;
			_logger = logger;
		}

		/// <summary>
		/// Get all sales orders
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<SalesOrderResponseDto>>> GetSalesOrders()
		{
			try
			{
				var salesOrders = await _salesOrderService.GetAllSalesOrdersAsync();
				return Ok(salesOrders);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting sales orders");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Get unprocessed sales orders
		/// </summary>
		[HttpGet("unprocessed")]
		public async Task<ActionResult<IEnumerable<SalesOrderResponseDto>>> GetUnprocessedSalesOrders()
		{
			try
			{
				var salesOrders = await _salesOrderService.GetAllSalesOrdersByProcessFlagAsync(0);
				return Ok(salesOrders);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting unprocessed sales orders");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Get processed sales orders
		/// </summary>
		[HttpGet("processed")]
		public async Task<ActionResult<IEnumerable<SalesOrderResponseDto>>> GetProcessedSalesOrders()
		{
			try
			{
				var salesOrders = await _salesOrderService.GetAllSalesOrdersByProcessFlagAsync(1);
				return Ok(salesOrders);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting processed sales orders");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Get sales order by ID
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<SalesOrderResponseDto>> GetSalesOrder(int id)
		{
			try
			{
				var salesOrder = await _salesOrderService.GetSalesOrderByIdAsync(id);
				if (salesOrder == null)
				{
					return NotFound($"Sales order with ID {id} not found");
				}
				return Ok(salesOrder);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting sales order {SalesOrderId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Search sales orders
		/// </summary>
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<SalesOrderResponseDto>>> SearchSalesOrders(
			[FromQuery] string? voucherNumber,
			[FromQuery] string? partyName,
			[FromQuery] DateTime? fromDate,
			[FromQuery] DateTime? toDate,
			[FromQuery] int? processFlag)
		{
			try
			{
				var searchDto = new SalesOrderSearchRequestDto
				{
					VoucherNumber = voucherNumber,
					PartyName = partyName,
					FromDate = fromDate,
					ToDate = toDate,
					ProcessFlag = processFlag
				};
				var salesOrders = await _salesOrderService.SearchSalesOrdersAsync(searchDto);
				return Ok(salesOrders);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while searching sales orders");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}


		/// <summary>
		/// Create a new sales order
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<SalesOrderResponseDto>> CreateSalesOrder(CreateSalesOrderRequestDto createSalesOrderDto)
		{
			try
			{
				var salesOrder = await _salesOrderService.CreateSalesOrderAsync(createSalesOrderDto);
				return CreatedAtAction(nameof(GetSalesOrder), new { id = salesOrder.Id }, salesOrder);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while creating sales order");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Update a sales order
		/// </summary>
		[HttpPut("{id}")]
		public async Task<ActionResult<SalesOrderResponseDto>> UpdateSalesOrder(int id, UpdateSalesOrderRequestDto updateSalesOrderDto)
		{
			try
			{
				var salesOrder = await _salesOrderService.UpdateSalesOrderAsync(id, updateSalesOrderDto);
				if (salesOrder == null)
				{
					return NotFound($"Sales order with ID {id} not found");
				}
				return Ok(salesOrder);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating sales order {SalesOrderId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Delete a sales order (soft delete)
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteSalesOrder(int id)
		{
			try
			{
				var result = await _salesOrderService.DeleteSalesOrderAsync(id);
				if (!result)
				{
					return NotFound($"Sales order with ID {id} not found");
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting sales order {SalesOrderId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Mark a sales order as processed
		/// </summary>
		[HttpPut("{id}/process")]
		public async Task<ActionResult> MarkAsProcessed(int id)
		{
			try
			{
				var result = await _salesOrderService.MarkAsProcessedAsync(id);
				if (!result)
				{
					return NotFound($"Sales order with ID {id} not found");
				}
				return Ok("Sales order marked as processed successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while marking sales order {SalesOrderId} as processed", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Mark a sales order item as processed
		/// </summary>
		[HttpPut("{salesOrderId}/items/{salesOrderItemId}/process")]
		public async Task<ActionResult> MarkSalesOrderItemAsProcessed(int salesOrderId, int salesOrderItemId)
		{
			try
			{
				var result = await _salesOrderService.MarkSalesOrderItemAsProcessedAsync(salesOrderId, salesOrderItemId);
				if (!result)
				{
					return NotFound($"Sales order item with ID {salesOrderItemId} not found in sales order {salesOrderId}");
				}
				return Ok("Sales order item marked as processed successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while marking sales order item {SalesOrderItemId} as processed", salesOrderItemId);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
	}
}