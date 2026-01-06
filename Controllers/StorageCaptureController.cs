using AvyyanBackend.Data;
using AvyyanBackend.DTOs.ProductionConfirmation;
using AvyyanBackend.DTOs.StorageCapture;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models.ProAllot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class StorageCaptureController : ControllerBase
	{
		private readonly IStorageCaptureService _storageCaptureService;
		private readonly ILogger<StorageCaptureController> _logger;
		private readonly ApplicationDbContext _context;

		public StorageCaptureController(IStorageCaptureService storageCaptureService, ILogger<StorageCaptureController> logger, ApplicationDbContext context)
		{
			_storageCaptureService = storageCaptureService;
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Get all storage captures
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<StorageCaptureResponseDto>>> GetStorageCaptures()
		{
			try
			{
				var storageCaptures = await _storageCaptureService.GetAllStorageCapturesAsync();
				return Ok(storageCaptures);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting storage captures");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}


		/// <summary>
		/// Search storage captures by various criteria
		/// </summary>
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<StorageCaptureResponseDto>>> SearchStorageCaptures(
			[FromQuery] string? lotNo,
			[FromQuery] string? fgRollNo,
			[FromQuery] string? locationCode,
			[FromQuery] string? tape,
			[FromQuery] string? customerName,
			[FromQuery] bool? isActive,
			[FromQuery] bool? isDispatched)
		{
			try
			{
				var searchDto = new StorageCaptureSearchRequestDto
				{
					LotNo = lotNo,
					FGRollNo = fgRollNo,
					LocationCode = locationCode,
					Tape = tape,
					CustomerName = customerName,
					IsActive = isActive,
					IsDispatched = isDispatched
				};
				var storageCaptures = await _storageCaptureService.SearchStorageCapturesAsync(searchDto);
				return Ok(storageCaptures);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while searching storage captures");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Get storage captures by multiple lot numbers
		/// </summary>
		[HttpGet("by-lots")]
		public async Task<ActionResult<IEnumerable<StorageCaptureResponseDto>>> GetStorageCapturesByLots([FromQuery] string lotNumbers)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(lotNumbers))
				{
					return BadRequest("Lot numbers parameter is required");
				}

				// Split comma-separated lot numbers
				var lotNumbersList = lotNumbers.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(l => l.Trim())
					.Where(l => !string.IsNullOrWhiteSpace(l))
					.ToList();

				if (!lotNumbersList.Any())
				{
					return BadRequest("No valid lot numbers provided");
				}

				_logger.LogInformation("Fetching storage captures for {Count} lot numbers", lotNumbersList.Count);

				var storageCaptures = await _storageCaptureService.GetStorageCapturesByLotNumbersAsync(lotNumbersList);
				return Ok(storageCaptures);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting storage captures by lot numbers");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}


		/// <summary>
		/// Create a new storage capture
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<StorageCaptureResponseDto>> CreateStorageCapture(CreateStorageCaptureRequestDto createStorageCaptureDto)
		{
			try
			{
				// Check if a storage capture with the same LotNo and FGRollNo already exists
				var existingStorageCapture = await _storageCaptureService.GetStorageCaptureByLotNoAndFGRollNoAsync(createStorageCaptureDto.LotNo, createStorageCaptureDto.FGRollNo);

				if (existingStorageCapture)
				{
					return BadRequest($"A storage capture with LotNo '{createStorageCaptureDto.LotNo}' and FGRollNo '{createStorageCaptureDto.FGRollNo}' already exists.");
				}


				var storageCapture = await _storageCaptureService.CreateStorageCaptureAsync(createStorageCaptureDto);
				return CreatedAtAction(nameof(GetStorageCaptures), new { id = storageCapture.Id }, storageCapture);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while creating storage capture");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
		/// <summary>
		/// Get roll confirmations by Allot ID
		///</summary>   
		// GET api/storagecapture/by-allot-id/{allotId}
		[HttpGet("by-allot-id/{allotId}")]
		public async Task<ActionResult<StorageCaptureRollDataResponseDto>> GetRollConfirmationsByAllotId(string allotId,int fgroll)
		{
			try
			{
				// Get only the first roll confirmation for the given AllotId
				var rollConfirmation = await _context.RollConfirmations
					.Where(r => r.AllotId == allotId && r.FgRollNo==fgroll && r.IsFGStickerGenerated==true)
					.FirstOrDefaultAsync();

				if (rollConfirmation == null)
				{
					return NotFound($"No roll confirmation found for Allot ID {allotId}.");
				}

				var productionAllotment = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.FirstOrDefaultAsync(pa => pa.AllotmentId == allotId);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with Allot ID {allotId} not found.");
				}

				// Map to DTOs - now with single roll confirmation
				var rollConfirmationDto = new RollConfirmationResponseDto
				{
					Id = rollConfirmation.Id,
					AllotId = rollConfirmation.AllotId,
					MachineName = rollConfirmation.MachineName,
					RollPerKg = rollConfirmation.RollPerKg,
					GreyGsm = rollConfirmation.GreyGsm,
					GreyWidth = rollConfirmation.GreyWidth,
					BlendPercent = rollConfirmation.BlendPercent,
					Cotton = rollConfirmation.Cotton,
					Polyester = rollConfirmation.Polyester,
					Spandex = rollConfirmation.Spandex,
					RollNo = rollConfirmation.RollNo,
					GrossWeight = rollConfirmation.GrossWeight,
					TareWeight = rollConfirmation.TareWeight,
					NetWeight = rollConfirmation.NetWeight,
					FgRollNo = rollConfirmation.FgRollNo,
					IsFGStickerGenerated = rollConfirmation.IsFGStickerGenerated,
					CreatedDate = rollConfirmation.CreatedDate
				};

				var machineAllocationDto = productionAllotment.MachineAllocations.Select(ma => new MachineAllocationDto
				{
					Id = ma.Id,
					ProductionAllotmentId = ma.ProductionAllotmentId,
					MachineName = ma.MachineName,
					MachineId = ma.MachineId,
					NumberOfNeedles = ma.NumberOfNeedles,
					Feeders = ma.Feeders,
					RPM = ma.RPM,
					RollPerKg = ma.RollPerKg,
					TotalLoadWeight = ma.TotalLoadWeight,
					TotalRolls = ma.TotalRolls,
					RollBreakdown = ma.RollBreakdown,
					EstimatedProductionTime = ma.EstimatedProductionTime
				}).FirstOrDefault();

				var productionAllotmentDto = new ProductionAllotmentDto
				{
					Id = productionAllotment.Id,
					AllotmentId = productionAllotment.AllotmentId,
					VoucherNumber = productionAllotment.VoucherNumber,
					ItemName = productionAllotment.ItemName,
					SalesOrderId = productionAllotment.SalesOrderId,
					SalesOrderItemId = productionAllotment.SalesOrderItemId,
					ActualQuantity = productionAllotment.ActualQuantity,
					YarnCount = productionAllotment.YarnCount,
					Diameter = productionAllotment.Diameter,
					Gauge = productionAllotment.Gauge,
					FabricType = productionAllotment.FabricType,
					SlitLine = productionAllotment.SlitLine,
					StitchLength = productionAllotment.StitchLength,
					Efficiency = productionAllotment.Efficiency,
					Composition = productionAllotment.Composition,
					TotalProductionTime = productionAllotment.TotalProductionTime,
					CreatedDate = productionAllotment.CreatedDate,
					YarnLotNo = productionAllotment.YarnLotNo,
					Counter = productionAllotment.Counter,
					ColourCode = productionAllotment.ColourCode,
					ReqGreyGsm = productionAllotment.ReqGreyGsm,
					ReqGreyWidth = productionAllotment.ReqGreyWidth,
					ReqFinishGsm = productionAllotment.ReqFinishGsm,
					ReqFinishWidth = productionAllotment.ReqFinishWidth,
					PartyName = productionAllotment.PartyName,
					TubeWeight = productionAllotment.TubeWeight,
					TapeColor = productionAllotment.TapeColor
				};

				var responseDto = new StorageCaptureRollDataResponseDto
				{
					RollConfirmation = rollConfirmationDto,
					MachineAllocation = machineAllocationDto ?? new MachineAllocationDto(),
					ProductionAllotment = productionAllotmentDto
				};

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching roll confirmation for Allot ID {AllotId}", allotId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		/// <summary>
		/// Update a storage capture
		/// </summary>
		[HttpPut("{id}")]
		public async Task<ActionResult<StorageCaptureResponseDto>> UpdateStorageCapture(int id, UpdateStorageCaptureRequestDto updateStorageCaptureDto)
		{
			try
			{
				var storageCapture = await _storageCaptureService.UpdateStorageCaptureAsync(id, updateStorageCaptureDto);
				if (storageCapture == null)
				{
					return NotFound($"Storage capture with ID {id} not found");
				}
				return Ok(storageCapture);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating storage capture {StorageCaptureId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Delete a storage capture (soft delete)
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteStorageCapture(int id)
		{
			try
			{
				var result = await _storageCaptureService.DeleteStorageCaptureAsync(id);
				if (!result)
				{
					return NotFound($"Storage capture with ID {id} not found");
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting storage capture {StorageCaptureId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
	}
}