using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.DTOs.SalesOrder;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Controllers
{
	/// <summary>
	/// Controller for managing Sales Order Web operations
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	[Produces("application/json")]
	public class SalesOrderWebController : ControllerBase
	{
		private readonly ISalesOrderWebService _salesOrderWebService;
		private readonly ILogger<SalesOrderWebController> _logger;

		public SalesOrderWebController(
			ISalesOrderWebService salesOrderWebService,
			ILogger<SalesOrderWebController> logger)
		{
			_salesOrderWebService = salesOrderWebService ?? throw new ArgumentNullException(nameof(salesOrderWebService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		#region Read Operations

		/// <summary>
		/// Get all sales orders web with optional filtering
		/// </summary>
		/// <param name="isProcess">Filter by process status (optional)</param>
		/// <param name="isJobWork">Filter by job work status (optional)</param>
		/// <param name="startDate">Filter by start date (optional)</param>
		/// <param name="endDate">Filter by end date (optional)</param>
		/// <returns>List of sales orders web</returns>
		/// <response code="200">Returns the list of sales orders web</response>
		/// <response code="500">Internal server error</response>
		[HttpGet]
		[ProducesResponseType(typeof(IEnumerable<SalesOrderWebResponseDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<IEnumerable<SalesOrderWebResponseDto>>> GetSalesOrdersWeb(
			[FromQuery] bool? isProcess = null,
			[FromQuery] bool? isJobWork = null,
			[FromQuery] DateTime? startDate = null,
			[FromQuery] DateTime? endDate = null)
		{
			try
			{
				_logger.LogInformation("Fetching sales orders web with filters - IsProcess: {IsProcess}, IsJobWork: {IsJobWork}, StartDate: {StartDate}, EndDate: {EndDate}",
					isProcess, isJobWork, startDate, endDate);

				var salesOrdersWeb = await _salesOrderWebService.GetAllAsync();

				// Apply filters if provided
				if (isProcess.HasValue)
				{
					salesOrdersWeb = salesOrdersWeb.Where(so => so.IsProcess == isProcess.Value);
				}

				if (isJobWork.HasValue)
				{
					salesOrdersWeb = salesOrdersWeb.Where(so => so.IsJobWork == isJobWork.Value);
				}

				if (startDate.HasValue)
				{
					salesOrdersWeb = salesOrdersWeb.Where(so => so.OrderDate >= startDate.Value);
				}

				if (endDate.HasValue)
				{
					salesOrdersWeb = salesOrdersWeb.Where(so => so.OrderDate <= endDate.Value);
				}

				var result = salesOrdersWeb.ToList();
				_logger.LogInformation("Successfully retrieved {Count} sales orders web", result.Count);

				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting sales orders web");
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		/// <summary>
		/// Get sales order web by ID with full details
		/// </summary>
		/// <param name="id">Sales order web ID</param>
		/// <returns>Sales order web details</returns>
		/// <response code="200">Returns the sales order web</response>
		/// <response code="404">Sales order web not found</response>
		/// <response code="500">Internal server error</response>
		[HttpGet("{id:int}")]
		[ProducesResponseType(typeof(SalesOrderWebResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<SalesOrderWebResponseDto>> GetSalesOrderWeb([Range(1, int.MaxValue)] int id)
		{
			try
			{
				_logger.LogInformation("Fetching sales order web with ID: {SalesOrderWebId}", id);

				var salesOrderWeb = await _salesOrderWebService.GetByIdAsync(id);

				if (salesOrderWeb == null)
				{
					_logger.LogWarning("Sales order web with ID {SalesOrderWebId} not found", id);
					return NotFound(new { message = $"Sales order web with ID {id} not found" });
				}

				_logger.LogInformation("Successfully retrieved sales order web with ID: {SalesOrderWebId}", id);
				return Ok(salesOrderWeb);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting sales order web {SalesOrderWebId}", id);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		/// <summary>
		/// Get sales order web by voucher number
		/// </summary>
		/// <param name="voucherNumber">Voucher number</param>
		/// <returns>Sales order web details</returns>
		/// <response code="200">Returns the sales order web</response>
		/// <response code="404">Sales order web not found</response>
		/// <response code="500">Internal server error</response>
		[HttpGet("by-voucher/{voucherNumber}")]
		[ProducesResponseType(typeof(SalesOrderWebResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<SalesOrderWebResponseDto>> GetSalesOrderWebByVoucherNumber(string voucherNumber)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(voucherNumber))
				{
					return BadRequest(new { message = "Voucher number is required" });
				}

				_logger.LogInformation("Fetching sales order web with voucher number: {VoucherNumber}", voucherNumber);

				var salesOrdersWeb = await _salesOrderWebService.GetAllAsync();
				var salesOrderWeb = salesOrdersWeb.FirstOrDefault(so =>
					so.VoucherNumber.Equals(voucherNumber, StringComparison.OrdinalIgnoreCase));

				if (salesOrderWeb == null)
				{
					_logger.LogWarning("Sales order web with voucher number {VoucherNumber} not found", voucherNumber);
					return NotFound(new { message = $"Sales order web with voucher number {voucherNumber} not found" });
				}

				_logger.LogInformation("Successfully retrieved sales order web with voucher number: {VoucherNumber}", voucherNumber);
				return Ok(salesOrderWeb);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting sales order web by voucher number {VoucherNumber}", voucherNumber);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		/// <summary>
		/// Get the next serial number for sales order items
		/// </summary>
		/// <returns>Next serial number</returns>
		/// <response code="200">Returns the next serial number</response>
		/// <response code="500">Internal server error</response>
		[HttpGet("next-serial-number")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<string>> GetNextSerialNumber()
		{
			try
			{
				_logger.LogInformation("Fetching next serial number");
				var serialNumber = await _salesOrderWebService.GetNextSerialNumberAsync();
				_logger.LogInformation("Next serial number: {SerialNumber}", serialNumber);
				return Ok(new { serialNumber });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting next serial number");
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		/// <summary>
		/// Get sales orders web count and statistics
		/// </summary>
		/// <returns>Statistics about sales orders web</returns>
		/// <response code="200">Returns statistics</response>
		/// <response code="500">Internal server error</response>
		[HttpGet("statistics")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> GetStatistics()
		{
			try
			{
				_logger.LogInformation("Fetching sales orders web statistics");

				var salesOrdersWeb = await _salesOrderWebService.GetAllAsync();
				var salesOrdersList = salesOrdersWeb.ToList();

				var statistics = new
				{
					totalOrders = salesOrdersList.Count,
					processedOrders = salesOrdersList.Count(so => so.IsProcess),
					pendingOrders = salesOrdersList.Count(so => !so.IsProcess),
					jobWorkOrders = salesOrdersList.Count(so => so.IsJobWork),
					totalAmount = salesOrdersList.Sum(so => so.TotalAmount),
					totalQuantity = salesOrdersList.Sum(so => so.TotalQuantity),
					ordersThisMonth = salesOrdersList.Count(so =>
						so.OrderDate.Month == DateTime.UtcNow.Month &&
						so.OrderDate.Year == DateTime.UtcNow.Year),
					ordersToday = salesOrdersList.Count(so => so.OrderDate.Date == DateTime.UtcNow.Date)
				};

				_logger.LogInformation("Successfully retrieved sales orders web statistics");
				return Ok(statistics);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting sales orders web statistics");
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		#endregion

		#region Create Operations

		/// <summary>
		/// Create a new sales order web
		/// </summary>
		/// <param name="createSalesOrderWebDto">Sales order web creation data</param>
		/// <returns>Created sales order web</returns>
		/// <response code="201">Sales order web created successfully</response>
		/// <response code="400">Invalid request data</response>
		/// <response code="500">Internal server error</response>
		[HttpPost]
		[ProducesResponseType(typeof(SalesOrderWebResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<SalesOrderWebResponseDto>> CreateSalesOrderWeb(
			[FromBody] CreateSalesOrderWebRequestDto createSalesOrderWebDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					_logger.LogWarning("Invalid model state for creating sales order web");
					return BadRequest(ModelState);
				}

				_logger.LogInformation("Creating new sales order web with voucher number: {VoucherNumber}",
					createSalesOrderWebDto.VoucherNumber);

				var salesOrderWeb = await _salesOrderWebService.CreateAsync(createSalesOrderWebDto);

				_logger.LogInformation("Successfully created sales order web with ID: {SalesOrderWebId}",
					salesOrderWeb.Id);

				return CreatedAtAction(
					nameof(GetSalesOrderWeb),
					new { id = salesOrderWeb.Id },
					salesOrderWeb);
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Invalid operation while creating sales order web");
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while creating sales order web");
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		#endregion

		#region Update Operations

		/// <summary>
		/// Update an existing sales order web
		/// </summary>
		/// <param name="id">Sales order web ID</param>
		/// <param name="updateSalesOrderWebDto">Sales order web update data</param>
		/// <returns>Updated sales order web</returns>
		/// <response code="200">Sales order web updated successfully</response>
		/// <response code="400">Invalid request data</response>
		/// <response code="404">Sales order web not found</response>
		/// <response code="500">Internal server error</response>
		[HttpPut("{id:int}")]
		[ProducesResponseType(typeof(SalesOrderWebResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<SalesOrderWebResponseDto>> UpdateSalesOrderWeb(
			[Range(1, int.MaxValue)] int id,
			[FromBody] UpdateSalesOrderWebRequestDto updateSalesOrderWebDto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					_logger.LogWarning("Invalid model state for updating sales order web {SalesOrderWebId}", id);
					return BadRequest(ModelState);
				}

				_logger.LogInformation("Updating sales order web with ID: {SalesOrderWebId}", id);

				var salesOrderWeb = await _salesOrderWebService.UpdateAsync(id, updateSalesOrderWebDto);

				if (salesOrderWeb == null)
				{
					_logger.LogWarning("Sales order web with ID {SalesOrderWebId} not found for update", id);
					return NotFound(new { message = $"Sales order web with ID {id} not found" });
				}

				_logger.LogInformation("Successfully updated sales order web with ID: {SalesOrderWebId}", id);
				return Ok(new
				{
					Message = "Successfully updated sales order web with ID: {SalesOrderWebId}",
					Data = salesOrderWeb
				});
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Invalid operation while updating sales order web {SalesOrderWebId}", id);
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating sales order web {SalesOrderWebId}", id);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		/// <summary>
		/// Mark a sales order web item as processed
		/// </summary>
		/// <param name="salesOrderWebId">Sales order web ID</param>
		/// <param name="salesOrderItemWebId">Sales order item web ID</param>
		/// <returns>Updated sales order web</returns>
		/// <response code="200">Item marked as processed successfully</response>
		/// <response code="404">Sales order web or item not found</response>
		/// <response code="500">Internal server error</response>
		[HttpPut("{salesOrderWebId:int}/items/{salesOrderItemWebId:int}/process")]
		[ProducesResponseType(typeof(SalesOrderWebResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<SalesOrderWebResponseDto>> MarkSalesOrderItemWebAsProcessed(
			[Range(1, int.MaxValue)] int salesOrderWebId,
			[Range(1, int.MaxValue)] int salesOrderItemWebId)
		{
			try
			{
				_logger.LogInformation("Marking sales order web item {SalesOrderItemWebId} as processed in order {SalesOrderWebId}",
					salesOrderItemWebId, salesOrderWebId);

				var result = await _salesOrderWebService.MarkSalesOrderItemWebAsProcessedAsync(
					salesOrderWebId, salesOrderItemWebId);

				if (result == null)
				{
					_logger.LogWarning("Sales order web item {SalesOrderItemWebId} not found in order {SalesOrderWebId}",
						salesOrderItemWebId, salesOrderWebId);
					return NotFound(new
					{
						message = $"Sales order web item with ID {salesOrderItemWebId} not found in sales order web {salesOrderWebId}"
					});
				}

				_logger.LogInformation("Successfully marked sales order web item {SalesOrderItemWebId} as processed",
					salesOrderItemWebId);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while marking sales order web item {SalesOrderItemWebId} as processed",
					salesOrderItemWebId);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		/// <summary>
		/// Mark entire sales order web as processed
		/// </summary>
		/// <param name="id">Sales order web ID</param>
		/// <returns>Updated sales order web</returns>
		/// <response code="200">Order marked as processed successfully</response>
		/// <response code="404">Sales order web not found</response>
		/// <response code="500">Internal server error</response>
		[HttpPut("{id:int}/process")]
		[ProducesResponseType(typeof(SalesOrderWebResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<SalesOrderWebResponseDto>> MarkSalesOrderWebAsProcessed(
			[Range(1, int.MaxValue)] int id)
		{
			try
			{
				_logger.LogInformation("Marking sales order web {SalesOrderWebId} as processed", id);

				var salesOrderWeb = await _salesOrderWebService.GetByIdAsync(id);

				if (salesOrderWeb == null)
				{
					_logger.LogWarning("Sales order web with ID {SalesOrderWebId} not found", id);
					return NotFound(new { message = $"Sales order web with ID {id} not found" });
				}

				// Create update DTO with IsProcess set to true
				var updateDto = new UpdateSalesOrderWebRequestDto
				{
					VoucherType = salesOrderWeb.VoucherType,
					VoucherNumber = salesOrderWeb.VoucherNumber,
					OrderDate = salesOrderWeb.OrderDate,
					TermsOfPayment = salesOrderWeb.TermsOfPayment,
					IsJobWork = salesOrderWeb.IsJobWork,
					SerialNo = salesOrderWeb.SerialNo,
					IsProcess = true, // Mark as processed
					CompanyName = salesOrderWeb.CompanyName,
					CompanyGSTIN = salesOrderWeb.CompanyGSTIN,
					CompanyState = salesOrderWeb.CompanyState,
					BuyerName = salesOrderWeb.BuyerName,
					BuyerGSTIN = salesOrderWeb.BuyerGSTIN,
					BuyerState = salesOrderWeb.BuyerState,
					BuyerPhone = salesOrderWeb.BuyerPhone,
					BuyerContactPerson = salesOrderWeb.BuyerContactPerson,
					BuyerAddress = salesOrderWeb.BuyerAddress,
					ConsigneeName = salesOrderWeb.ConsigneeName,
					ConsigneeGSTIN = salesOrderWeb.ConsigneeGSTIN,
					ConsigneeState = salesOrderWeb.ConsigneeState,
					ConsigneePhone = salesOrderWeb.ConsigneePhone,
					ConsigneeContactPerson = salesOrderWeb.ConsigneeContactPerson,
					ConsigneeAddress = salesOrderWeb.ConsigneeAddress,
					Remarks = salesOrderWeb.Remarks,
					OtherReference = salesOrderWeb.OtherReference,
					OrderNo = salesOrderWeb.OrderNo,
					TermsOfDelivery = salesOrderWeb.TermsOfDelivery,
					DispatchThrough = salesOrderWeb.DispatchThrough,
					TotalQuantity = salesOrderWeb.TotalQuantity,
					TotalAmount = salesOrderWeb.TotalAmount,
					Items = salesOrderWeb.Items.Select(item => new UpdateSalesOrderItemWebRequestDto
					{
						Id = item.Id,
						ItemName = item.ItemName,
						HSNCode = item.HSNCode,
						YarnCount = item.YarnCount,
						Dia = item.Dia,
						GG = item.GG,
						FabricType = item.FabricType,
						Composition = item.Composition,
						WtPerRoll = item.WtPerRoll,
						NoOfRolls = item.NoOfRolls,
						Rate = item.Rate,
						Qty = item.Qty,
						Amount = item.Amount,
						IGST = item.IGST,
						SGST = item.SGST,
						CGST = item.CGST,
						Remarks = item.Remarks,
						Unit = item.Unit,
						SlitLine = item.SlitLine,
						StitchLength = item.StitchLength,
						DueDate = item.DueDate,
						IsProcess = item.IsProcess
					}).ToList()
				};

				var result = await _salesOrderWebService.UpdateAsync(id, updateDto);

				_logger.LogInformation("Successfully marked sales order web {SalesOrderWebId} as processed", id);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while marking sales order web {SalesOrderWebId} as processed", id);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		#endregion

		#region Delete Operations

		/// <summary>
		/// Delete a sales order web
		/// </summary>
		/// <param name="id">Sales order web ID</param>
		/// <returns>No content on success</returns>
		/// <response code="204">Sales order web deleted successfully</response>
		/// <response code="404">Sales order web not found</response>
		/// <response code="500">Internal server error</response>
		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> DeleteSalesOrderWeb([Range(1, int.MaxValue)] int id)
		{
			try
			{
				_logger.LogInformation("Deleting sales order web with ID: {SalesOrderWebId}", id);

				var result = await _salesOrderWebService.DeleteAsync(id);

				if (!result)
				{
					_logger.LogWarning("Sales order web with ID {SalesOrderWebId} not found for deletion", id);
					return NotFound(new { message = $"Sales order web with ID {id} not found" });
				}

				_logger.LogInformation("Successfully deleted sales order web with ID: {SalesOrderWebId}", id);
				return NoContent();
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Cannot delete sales order web {SalesOrderWebId}", id);
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting sales order web {SalesOrderWebId}", id);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		/// <summary>
		/// Bulk delete sales orders web
		/// </summary>
		/// <param name="ids">List of sales order web IDs to delete</param>
		/// <returns>Result of bulk deletion</returns>
		/// <response code="200">Bulk deletion completed</response>
		/// <response code="400">Invalid request data</response>
		/// <response code="500">Internal server error</response>
		[HttpDelete("bulk")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> BulkDeleteSalesOrdersWeb([FromBody] List<int> ids)
		{
			try
			{
				if (ids == null || !ids.Any())
				{
					return BadRequest(new { message = "No IDs provided for deletion" });
				}

				_logger.LogInformation("Bulk deleting {Count} sales orders web", ids.Count);

				var deletedCount = 0;
				var failedIds = new List<int>();

				foreach (var id in ids)
				{
					try
					{
						var result = await _salesOrderWebService.DeleteAsync(id);
						if (result)
						{
							deletedCount++;
						}
						else
						{
							failedIds.Add(id);
						}
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Failed to delete sales order web {SalesOrderWebId}", id);
						failedIds.Add(id);
					}
				}

				_logger.LogInformation("Bulk deletion completed. Deleted: {DeletedCount}, Failed: {FailedCount}",
					deletedCount, failedIds.Count);

				return Ok(new
				{
					message = "Bulk deletion completed",
					deletedCount,
					failedCount = failedIds.Count,
					failedIds
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during bulk deletion");
				return StatusCode(StatusCodes.Status500InternalServerError,
					new { message = "An error occurred while processing your request", error = ex.Message });
			}
		}

		#endregion
	}
}