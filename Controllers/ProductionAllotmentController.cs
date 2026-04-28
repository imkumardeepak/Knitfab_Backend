using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.ProAllotDto;
using AvyyanBackend.Models.ProAllot;
using AvyyanBackend.Models.ProductionConfirmation;
using AvyyanBackend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AvyyanBackend.Controllers
{
	// Add the DTO class for updating status
	public class UpdateStatusRequest
	{
		public int Status { get; set; }
	}

	[ApiController]
	[Route("api/[controller]")]
	public class ProductionAllotmentController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<ProductionAllotmentController> _logger;
		private readonly IMapper _mapper;
		private readonly IConfiguration _configuration;
		public ProductionAllotmentController(ApplicationDbContext context, ILogger<ProductionAllotmentController> logger, IMapper mapper, IConfiguration configuration)
		{
			_context = context;
			_logger = logger;
			_mapper = mapper;
			_configuration = configuration;
		}

		// GET api/productionallotment/next-serial-number
		[HttpGet("next-serial-number")]
		public async Task<ActionResult<string>> GetNextSerialNumber()
		{
			try
			{
				// Get the maximum serial number from existing production allotments and add 1
				var maxSerialNumber = 0;
				var existingAllotments = await _context.ProductionAllotments
					.Select(pa => pa.SerialNo)
					.ToListAsync();

				if (existingAllotments.Any())
				{
					foreach (var serialNo in existingAllotments)
					{
						// Parse the serial number (format: "0001", "0002", etc.)
						if (int.TryParse(serialNo, out int serial))
						{
							if (serial > maxSerialNumber)
								maxSerialNumber = serial;
						}
					}
				}

				var nextNumber = maxSerialNumber + 1;

				// Format as 4-digit zero-padded string
				var serialNumber = nextNumber.ToString("D4");

				return Ok(serialNumber);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating next serial number");
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/productionallotment/by-allot-id/{allotId}
		[HttpGet("by-allot-id/{allotId}")]
		public async Task<ActionResult<ProductionAllotmentResponseDto>> GetProductionAllotmentByAllotId(string allotId)
		{
			try
			{
				var productionAllotment = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.FirstOrDefaultAsync(pa => pa.AllotmentId == allotId);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with ID {allotId} not found.");
				}

				var responseDto = new ProductionAllotmentResponseDto
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
					TotalProductionTime = productionAllotment.MachineAllocations.Sum(ma => ma.EstimatedProductionTime),
					CreatedDate = productionAllotment.CreatedDate,
					YarnLotNo = productionAllotment.YarnLotNo,
					Counter = productionAllotment.Counter,
					ColourCode = productionAllotment.ColourCode,
					ReqGreyGsm = productionAllotment.ReqGreyGsm,
					ReqGreyWidth = productionAllotment.ReqGreyWidth,
					ReqFinishGsm = productionAllotment.ReqFinishGsm,
					ReqFinishWidth = productionAllotment.ReqFinishWidth,
					YarnPartyName = productionAllotment.YarnPartyName, // New field for yarn party name
					PolybagColor = productionAllotment.PolybagColor, // New field for polybag color
					PartyName = productionAllotment.PartyName,
					OtherReference = productionAllotment.OtherReference,
					TubeWeight = productionAllotment.TubeWeight,
					ShrinkRapWeight = productionAllotment.ShrinkRapWeight,
					TotalWeight = productionAllotment.TotalWeight,
					TapeColor = productionAllotment.TapeColor,
					SerialNo = productionAllotment.SerialNo,
					ProductionStatus = productionAllotment.ProductionStatus,
					IsOnHold = productionAllotment.ProductionStatus == 1,
					IsSuspended = productionAllotment.ProductionStatus == 2,
					MachineAllocations = productionAllotment.MachineAllocations.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList()
				};

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching production allotment by AllotmentId: {AllotmentId}", allotId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/productionallotment
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductionAllotmentResponseDto>>> GetAllProductionAllotments()
		{
			try
			{
				var productionAllotments = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.ToListAsync();

				var responseDtos = productionAllotments.Select(pa => new ProductionAllotmentResponseDto
				{
					Id = pa.Id,
					AllotmentId = pa.AllotmentId,
					VoucherNumber = pa.VoucherNumber,
					ItemName = pa.ItemName,
					SalesOrderId = pa.SalesOrderId,
					SalesOrderItemId = pa.SalesOrderItemId,
					ActualQuantity = pa.ActualQuantity,
					YarnCount = pa.YarnCount,
					Diameter = pa.Diameter,
					Gauge = pa.Gauge,
					FabricType = pa.FabricType,
					SlitLine = pa.SlitLine,
					StitchLength = pa.StitchLength,
					Efficiency = pa.Efficiency,
					Composition = pa.Composition,
					TotalProductionTime = pa.TotalProductionTime,
					CreatedDate = pa.CreatedDate,
					YarnLotNo = pa.YarnLotNo,
					Counter = pa.Counter,
					ColourCode = pa.ColourCode,
					ReqGreyGsm = pa.ReqGreyGsm,
					ReqGreyWidth = pa.ReqGreyWidth,
					ReqFinishGsm = pa.ReqFinishGsm,
					ReqFinishWidth = pa.ReqFinishWidth,
					YarnPartyName = pa.YarnPartyName, // New field for yarn party name
					PolybagColor = pa.PolybagColor, // New field for polybag color
					PartyName = pa.PartyName,
					OtherReference = pa.OtherReference,
					TubeWeight = pa.TubeWeight,
					ShrinkRapWeight = pa.ShrinkRapWeight,
					TotalWeight = pa.TotalWeight,
					TapeColor = pa.TapeColor,
					SerialNo = pa.SerialNo,
					ProductionStatus = pa.ProductionStatus,
					IsOnHold = pa.ProductionStatus == 1,
					IsSuspended = pa.ProductionStatus == 2,
					MachineAllocations = pa.MachineAllocations.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList()
				}).ToList();

				return Ok(responseDtos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching production allotments");
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// Add this new DTO for search requests
		public class ProductionAllotmentSearchRequestDto
		{
			public string VoucherNumber { get; set; }
			public DateTime? FromDate { get; set; }
			public DateTime? ToDate { get; set; }
		}

		// Add this new endpoint after the GetAllProductionAllotments method
		// GET api/productionallotment/search
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<ProductionAllotmentResponseDto>>> SearchProductionAllotments(
			[FromQuery] string voucherNumber = null,
			[FromQuery] DateTime? fromDate = null,
			[FromQuery] DateTime? toDate = null)
		{
			try
			{
				var query = _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.AsQueryable();

				// Apply voucher number filter if provided
				if (!string.IsNullOrEmpty(voucherNumber))
				{
					query = query.Where(pa => pa.VoucherNumber.Contains(voucherNumber));
				}

				// Apply date filters if provided
				if (fromDate.HasValue)
				{
					query = query.Where(pa => pa.CreatedDate >= fromDate.Value);
				}

				if (toDate.HasValue)
				{
					// Include the entire end date by setting time to end of day
					var endDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
					query = query.Where(pa => pa.CreatedDate <= endDate);
				}

				var productionAllotments = await query.ToListAsync();

				var responseDtos = productionAllotments.Select(pa => new ProductionAllotmentResponseDto
				{
					Id = pa.Id,
					AllotmentId = pa.AllotmentId,
					VoucherNumber = pa.VoucherNumber,
					ItemName = pa.ItemName,
					SalesOrderId = pa.SalesOrderId,
					SalesOrderItemId = pa.SalesOrderItemId,
					ActualQuantity = pa.ActualQuantity,
					YarnCount = pa.YarnCount,
					Diameter = pa.Diameter,
					Gauge = pa.Gauge,
					FabricType = pa.FabricType,
					SlitLine = pa.SlitLine,
					StitchLength = pa.StitchLength,
					Efficiency = pa.Efficiency,
					Composition = pa.Composition,
					TotalProductionTime = pa.MachineAllocations.Sum(ma => ma.EstimatedProductionTime),
					CreatedDate = pa.CreatedDate,
					YarnLotNo = pa.YarnLotNo,
					Counter = pa.Counter,
					ColourCode = pa.ColourCode,
					ReqGreyGsm = pa.ReqGreyGsm,
					ReqGreyWidth = pa.ReqGreyWidth,
					ReqFinishGsm = pa.ReqFinishGsm,
					ReqFinishWidth = pa.ReqFinishWidth,
					PartyName = pa.PartyName,
					OtherReference = pa.OtherReference,
					TubeWeight = pa.TubeWeight,
					ShrinkRapWeight = pa.ShrinkRapWeight,
					TotalWeight = pa.TotalWeight,
					TapeColor = pa.TapeColor,
					SerialNo = pa.SerialNo,
					ProductionStatus = pa.ProductionStatus,
					IsOnHold = pa.ProductionStatus == 1,
					IsSuspended = pa.ProductionStatus == 2,
					MachineAllocations = pa.MachineAllocations.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList()
				});

				return Ok(responseDtos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error searching production allotments");
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}


		////  Sticker Printing Logic
		[HttpPost("stkprint/{id}")]
		public IActionResult stkprint(int id) // MachineAllocation id
		{
			try
			{
				// Get the specific machine allocation with its parent production allotment
				var machineAllocation = _context.MachineAllocations
					.Include(ma => ma.ProductionAllotment)
					.FirstOrDefault(ma => ma.Id == id);

				if (machineAllocation == null)
				{
					return NotFound($"Machine allocation with ID {id} not found.");
				}

				string filepath = Path.Combine("wwwroot", "Sticker", "MLRoll.prn");
				string printerName = _configuration["Printers:Printer_IP"];

				if (!System.IO.File.Exists(filepath))
				{
					return StatusCode(500, "PRN template file not found.");
				}

				// Read the PRN file content
				string fileContent = System.IO.File.ReadAllText(filepath);

				// Parse roll breakdown to determine roll numbers
				var rollBreakdown = JsonConvert.DeserializeObject<DTOs.ProAllotDto.RollBreakdown>(machineAllocation.RollBreakdown);
				int totalRolls = (int)machineAllocation.TotalRolls;

				// Generate QR codes for each roll
				for (int rollNumber = 1; rollNumber <= totalRolls; rollNumber++)
				{
					string currentFileContent = fileContent;

					// Replace placeholders with actual values
					currentFileContent = currentFileContent
						.Replace("<MCCODE>", machineAllocation.MachineName.Trim())
						.Replace("<LCODE>", machineAllocation.ProductionAllotment.AllotmentId.Trim())
						.Replace("<ROLLNO>", rollNumber.ToString())
						.Replace("<YCOUNT>", machineAllocation.ProductionAllotment.YarnCount?.Trim() ?? "")
						.Replace("<DIAGG>", $"{machineAllocation.ProductionAllotment.Diameter} X {machineAllocation.ProductionAllotment.Gauge}")
						.Replace("<STICHLEN>", machineAllocation.ProductionAllotment.StitchLength.ToString())
						.Replace("<FEBTYP>", machineAllocation.ProductionAllotment.FabricType?.Trim() ?? "")
						.Replace("<COMP>", machineAllocation.ProductionAllotment.Composition?.Trim() ?? "");

					// Send to printer
					PrintToNetworkPrinter(printerName, currentFileContent);
				}

				return Ok(new { message = $"{totalRolls} QR codes printed successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error printing QR codes: {ex.Message}");
			}
		}

		// New endpoint to generate stickers for specific roll numbers based on roll assignment
		[HttpPost("stkprint/roll-assignment/{id}")]
		public IActionResult GenerateStickersForRollAssignment(int id, [FromBody] GenerateStickersForRollsRequest request)
		{
			try
			{
				var rollAssignment = _context.RollAssignments
					.Include(ra => ra.MachineAllocation)
					.ThenInclude(ma => ma.ProductionAllotment)
					.FirstOrDefault(ra => ra.Id == id);

				if (rollAssignment == null)
					return NotFound($"Roll assignment with ID {id} not found.");

				if (request.RollNumbers == null || request.RollNumbers.Count == 0)
					return BadRequest("Roll numbers must be provided.");

				string doubleStickerPath = Path.Combine("wwwroot", "Sticker", "MLRoll(2ups)5025.prn");
				string singleStickerPath = Path.Combine("wwwroot", "Sticker", "MLRoll.prn");

				string printerName = _configuration["Printers:PrinterName"] ?? "";

				if (string.IsNullOrEmpty(printerName))
					return StatusCode(500, "Printer name not configured.");

				if (!System.IO.File.Exists(doubleStickerPath) || !System.IO.File.Exists(singleStickerPath))
					return StatusCode(500, "PRN template file not found.");

				string doubleStickerContent = System.IO.File.ReadAllText(doubleStickerPath);
				string singleStickerContent = System.IO.File.ReadAllText(singleStickerPath);

				var pa = rollAssignment.MachineAllocation.ProductionAllotment;


				for (int i = 0; i < request.RollNumbers.Count; i += 2)
				{
					if (i + 1 < request.RollNumbers.Count)
					{
						// We have a pair of rolls: left = rollNo1, right = rollNo2
						int rollNo1 = request.RollNumbers[i];
						int rollNo2 = request.RollNumbers[i + 1];

						string fileContent = doubleStickerContent
							// --- Left label (1) ---
							.Replace("<MCCODE1>", rollAssignment.MachineAllocation.MachineName.Trim())
							.Replace("<LCODE1>", pa.AllotmentId.Trim())
							.Replace("<ROLLNO1>", rollNo1.ToString())
							.Replace("<YCOUNT1>", pa.YarnCount?.Trim() ?? "")
							.Replace("<DIAGG1>", $"{pa.Diameter} X {pa.Gauge}")
							.Replace("<STICHLEN1>", pa.StitchLength.ToString())
							.Replace("<FEBTYP1>", pa.FabricType?.Trim() ?? "")
							.Replace("<COMP1>", pa.Composition?.Trim() ?? "")

							// --- Right label (2) ---
							.Replace("<MCCODE2>", rollAssignment.MachineAllocation.MachineName.Trim())
							.Replace("<LCODE2>", pa.AllotmentId.Trim())
							.Replace("<ROLLNO2>", rollNo2.ToString())
							.Replace("<YCOUNT2>", pa.YarnCount?.Trim() ?? "")
							.Replace("<DIAGG2>", $"{pa.Diameter} X {pa.Gauge}")
							.Replace("<STICHLEN2>", pa.StitchLength.ToString())
							.Replace("<FEBTYP2>", pa.FabricType?.Trim() ?? "")
							.Replace("<COMP2>", pa.Composition?.Trim() ?? "");

						PrintToUSB(printerName, fileContent);
						_logger.LogInformation($"Stickers for rolls {rollNo1} and {rollNo2} printed.");
					}
					else
					{
						// One leftover roll (odd count) ? single sticker
						int rollNo = request.RollNumbers[i];

						string fileContent = singleStickerContent
							.Replace("<MCCODE>", rollAssignment.MachineAllocation.MachineName.Trim())
							.Replace("<LCODE>", pa.AllotmentId.Trim())
							.Replace("<ROLLNO>", rollNo.ToString())
							.Replace("<YCOUNT>", pa.YarnCount?.Trim() ?? "")
							.Replace("<DIAGG>", $"{pa.Diameter} X {pa.Gauge}")
							.Replace("<STICHLEN>", pa.StitchLength.ToString())
							.Replace("<FEBTYP>", pa.FabricType?.Trim() ?? "")
							.Replace("<COMP>", pa.Composition?.Trim() ?? "");

						PrintToUSB(printerName, fileContent);
						_logger.LogInformation($"Sticker for roll {rollNo} printed.");
					}
				}
				return Ok(new { message = $"{request.RollNumbers.Count} QR codes printed successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error printing QR codes: {ex.Message}");
			}
		}


		// New endpoint for FG Roll Sticker Printing
		[HttpPost("fgsticker/{id}")]
		public IActionResult PrintFGRollSticker(int id) // RollConfirmation id
		{
			try
			{
				// Get the specific roll confirmation
				var rollConfirmation = _context.RollConfirmations
					.FirstOrDefault(rc => rc.Id == id);

				if (rollConfirmation == null)
				{
					return NotFound($"Roll confirmation with ID {id} not found.");
				}

				// Get the related production allotment
				var productionAllotment = _context.ProductionAllotments
					.FirstOrDefault(pa => pa.AllotmentId == rollConfirmation.AllotId);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with AllotId {rollConfirmation.AllotId} not found.");
				}

				string filepath = Path.Combine("wwwroot", "Sticker", "FGRoll.prn");
				string printerName = _configuration["Printers:FG_Printer_IP"];

				if (!System.IO.File.Exists(filepath))
				{
					return StatusCode(500, "FG Roll PRN template file not found.");
				}

				// Read the PRN file content
				string fileContent = System.IO.File.ReadAllText(filepath);

				// Get company name from SalesOrderWeb
				string companyName = "AVYAAN KNITFAB"; // Default company name
				
				// Try to get SalesOrderWeb to fetch company name
				var salesOrderWeb = _context.SalesOrdersWeb
					.FirstOrDefault(sow => sow.Id == productionAllotment.SalesOrderId);
				
				// Use company name from SalesOrderWeb if available and matches "Avyaan Knitfab" (case insensitive)
				if (salesOrderWeb != null && !string.IsNullOrWhiteSpace(salesOrderWeb.CompanyName))
				{
					// Check if company name contains "Avyaan Knitfab" (case insensitive)
					if (salesOrderWeb.CompanyName.IndexOf("Avyaan Knitfab", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						companyName = "AVYAAN KNITFAB"; // Print in uppercase
					}
					else
					{
						companyName = ""; // Don't print any company name
					}
				}
				else
				{
					// If no company name found, don't print any company name
					companyName = "";
				}

				// Prepare customer name (split at word boundaries for the two customer fields)
				// Use OtherReference if available, otherwise use PartyName
				string customerName = !string.IsNullOrWhiteSpace(productionAllotment.OtherReference) 
					? productionAllotment.OtherReference.Trim() 
					: productionAllotment.PartyName?.Trim() ?? "";
				string customer1 = customerName;
				string customer2 = "";

				// If customer name is long, split it between the two fields at word boundaries
				if (customerName.Length > 20)
				{
					// Find the last space within the first 20 characters
					int splitIndex = customerName.Substring(0, 20).LastIndexOf(' ');

					// If no space found or it's at the beginning, split at position 20
					if (splitIndex <= 0)
					{
						splitIndex = 20;
					}

					customer1 = customerName.Substring(0, splitIndex).Trim();
					customer2 = customerName.Substring(splitIndex).Trim();
				}

				// Prepare yarn count (split at word boundaries for the two yarn count fields)
				string yarnCount = productionAllotment.YarnCount?.Trim() ?? "";
				string yarnCount1 = yarnCount;
				string yarnCount2 = "";

				// If yarn count is long, split it between the two fields at word boundaries
				if (yarnCount.Length > 20)
				{
					// Find the last space within the first 20 characters
					int splitIndex = yarnCount.Substring(0, 20).LastIndexOf(' ');

					// If no space found or it's at the beginning, split at position 20
					if (splitIndex <= 0)
					{
						splitIndex = 20;
					}

					yarnCount1 = yarnCount.Substring(0, splitIndex).Trim();
					yarnCount2 = yarnCount.Substring(splitIndex).Trim();
				}

				// Prepare fabric type (split at word boundaries for the two fabric type fields)
				string fabricType = productionAllotment.FabricType?.Trim() ?? "";
				string fabricType1 = fabricType;
				string fabricType2 = "";

				// If fabric type is long, split it between the two fields at word boundaries
				// As per requirement, split first 9 characters for fabricType1 and remaining for fabricType2
				if (fabricType.Length > 9)
				{
					fabricType1 = fabricType.Substring(0, 9).Trim();
					fabricType2 = fabricType.Substring(9).Trim();
				}

				// Replace placeholders with actual values from roll confirmation and related data
				string currentFileContent = fileContent
					.Replace("<CompanyName>", companyName)
					.Replace("<CUSTOMER1>", customer1)
					.Replace("<CUSTOMER2>", customer2)
					.Replace("<MCCODE>", rollConfirmation.MachineName.Trim())
					.Replace("<YCOUNT>", yarnCount)
					.Replace("<YARNCOUNT1>", yarnCount1)
					.Replace("<YARNCOUNT2>", yarnCount2)
					.Replace("<DIAGG>", $"{productionAllotment.Diameter} X {productionAllotment.Gauge}")
					.Replace("<STICHLEN>", productionAllotment.StitchLength.ToString())
					.Replace("<FGSM>", rollConfirmation.GreyGsm.ToString("F2"))
					.Replace("<WIDTH>", rollConfirmation.GreyWidth.ToString("F2"))
					.Replace("<SLITLINE>", productionAllotment.SlitLine?.Trim() ?? "")
					.Replace("<TAPE>", productionAllotment.TapeColor?.Trim() ?? "")
					.Replace("<GROSSWT>", rollConfirmation.GrossWeight?.ToString("F2") ?? "")
					.Replace("<NETWT>", rollConfirmation.NetWeight?.ToString("F2") ?? "")
					.Replace("<LCODE>", rollConfirmation.AllotId.Trim())
					.Replace("<ROLLNO>", rollConfirmation.RollNo.Trim())
					.Replace("<FGROLLNO>", rollConfirmation.FgRollNo?.ToString() ?? rollConfirmation.RollNo.Trim()) // Use FG Roll No if available
					.Replace("<FEBTYP>", fabricType) // Full fabric type
					.Replace("<FEBTYP1>", fabricType1) // First part of fabric type
					.Replace("<FEBTYP2>", fabricType2) // Second part of fabric type
					.Replace("<COMP>", productionAllotment.Composition?.Trim() ?? "");

				// Send to printer
				PrintToNetworkPrinter(printerName, currentFileContent);

				return Ok(new { message = "FG Roll sticker printed successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error printing FG Roll sticker: {ex.Message}");
			}
		}

		// POST api/productionallotment/fgsticker/bulk - Print FG Roll stickers for multiple roll confirmations
		[HttpPost("fgsticker/bulk")]
		public IActionResult PrintFGRollStickersBulk([FromBody] int[] rollConfirmationIds)
		{
			try
			{
				if (rollConfirmationIds == null || rollConfirmationIds.Length == 0)
				{
					return BadRequest("No roll confirmation IDs provided.");
				}

				var results = new List<object>();
				var successCount = 0;
				var errorCount = 0;

				foreach (var id in rollConfirmationIds)
				{
					try
					{
						// Get the specific roll confirmation
						var rollConfirmation = _context.RollConfirmations
							.FirstOrDefault(rc => rc.Id == id);

						if (rollConfirmation == null)
						{
							results.Add(new { id, success = false, message = $"Roll confirmation with ID {id} not found." });
							errorCount++;
							continue;
						}

						// Get the related production allotment
						var productionAllotment = _context.ProductionAllotments
							.FirstOrDefault(pa => pa.AllotmentId == rollConfirmation.AllotId);

						if (productionAllotment == null)
						{
							results.Add(new { id, success = false, message = $"Production allotment with AllotId {rollConfirmation.AllotId} not found." });
							errorCount++;
							continue;
						}

						string filepath = Path.Combine("wwwroot", "Sticker", "FGRoll.prn");
						string printerName = _configuration["Printers:FG_Printer_IP"];

						if (!System.IO.File.Exists(filepath))
						{
							results.Add(new { id, success = false, message = "FG Roll PRN template file not found." });
							errorCount++;
							continue;
						}

						// Read the PRN file content
						string fileContent = System.IO.File.ReadAllText(filepath);

						// Get company name from SalesOrderWeb
						string companyName = "AVYAAN KNITFAB"; // Default company name
						
						// Try to get SalesOrderWeb to fetch company name
						var salesOrderWeb = _context.SalesOrdersWeb
							.FirstOrDefault(sow => sow.Id == productionAllotment.SalesOrderId);
						

						
						// Use company name from SalesOrderWeb if available and matches "Avyaan Knitfab" (case insensitive)
						if (salesOrderWeb != null && !string.IsNullOrWhiteSpace(salesOrderWeb.CompanyName))
						{
							// Check if company name contains "Avyaan Knitfab" (case insensitive)
							if (salesOrderWeb.CompanyName.IndexOf("Avyaan Knitfab", StringComparison.OrdinalIgnoreCase) >= 0)
							{
								companyName = "AVYAAN KNITFAB"; // Print in uppercase
							}
							else
							{
								companyName = ""; // Don't print any company name
							}
						}
						else
						{
							// If no company name found, don't print any company name
							companyName = "";
						}

						// Prepare customer name (split at word boundaries for the two customer fields)
						// Use OtherReference if available, otherwise use PartyName
						string customerName = !string.IsNullOrWhiteSpace(productionAllotment.OtherReference) 
							? productionAllotment.OtherReference.Trim() 
							: productionAllotment.PartyName?.Trim() ?? "";
						string customer1 = customerName;
						string customer2 = "";

						// If customer name is long, split it between the two fields at word boundaries
						if (customerName.Length > 20)
						{
							// Find the last space within the first 20 characters
							int splitIndex = customerName.Substring(0, 20).LastIndexOf(' ');

							// If no space found or it's at the beginning, split at position 20
							if (splitIndex <= 0)
							{
								splitIndex = 20;
							}

							customer1 = customerName.Substring(0, splitIndex).Trim();
							customer2 = customerName.Substring(splitIndex).Trim();
						}

						// Prepare yarn count (split at word boundaries for the two yarn count fields)
						string yarnCount = productionAllotment.YarnCount?.Trim() ?? "";
						string yarnCount1 = yarnCount;
						string yarnCount2 = "";

						// If yarn count is long, split it between the two fields at word boundaries
						if (yarnCount.Length > 20)
						{
							// Find the last space within the first 20 characters
							int splitIndex = yarnCount.Substring(0, 20).LastIndexOf(' ');

							// If no space found or it's at the beginning, split at position 20
							if (splitIndex <= 0)
							{
								splitIndex = 20;
							}

							yarnCount1 = yarnCount.Substring(0, splitIndex).Trim();
							yarnCount2 = yarnCount.Substring(splitIndex).Trim();
						}

						// Prepare fabric type (split at word boundaries for the two fabric type fields)
						string fabricType = productionAllotment.FabricType?.Trim() ?? "";
						string fabricType1 = fabricType;
						string fabricType2 = "";

						// If fabric type is long, split it between the two fields at word boundaries
						// As per requirement, split first 9 characters for fabricType1 and remaining for fabricType2
						if (fabricType.Length > 9)
						{
							fabricType1 = fabricType.Substring(0, 9).Trim();
							fabricType2 = fabricType.Substring(9).Trim();
						}

						// Replace placeholders with actual values from roll confirmation and related data
						string currentFileContent = fileContent
							.Replace("<CompanyName>", companyName)
							.Replace("<CUSTOMER1>", customer1)
							.Replace("<CUSTOMER2>", customer2)
							.Replace("<MCCODE>", rollConfirmation.MachineName.Trim())
							.Replace("<YCOUNT>", yarnCount)
							.Replace("<YARNCOUNT1>", yarnCount1)
							.Replace("<YARNCOUNT2>", yarnCount2)
							.Replace("<DIAGG>", $"{productionAllotment.Diameter} X {productionAllotment.Gauge}")
							.Replace("<STICHLEN>", productionAllotment.StitchLength.ToString())
							.Replace("<FGSM>", rollConfirmation.GreyGsm.ToString("F2"))
							.Replace("<WIDTH>", rollConfirmation.GreyWidth.ToString("F2"))
							.Replace("<SLITLINE>", productionAllotment.SlitLine?.Trim() ?? "")
							.Replace("<TAPE>", productionAllotment.TapeColor?.Trim() ?? "")
							.Replace("<GROSSWT>", rollConfirmation.GrossWeight?.ToString("F2") ?? "")
							.Replace("<NETWT>", rollConfirmation.NetWeight?.ToString("F2") ?? "")
							.Replace("<LCODE>", rollConfirmation.AllotId.Trim())
							.Replace("<ROLLNO>", rollConfirmation.RollNo.Trim())
							.Replace("<FGROLLNO>", rollConfirmation.FgRollNo?.ToString() ?? rollConfirmation.RollNo.Trim()) // Use FG Roll No if available
							.Replace("<FEBTYP>", fabricType) // Full fabric type
							.Replace("<FEBTYP1>", fabricType1) // First part of fabric type
							.Replace("<FEBTYP2>", fabricType2) // Second part of fabric type
							.Replace("<COMP>", productionAllotment.Composition?.Trim() ?? "");

						// Send to printer
						PrintToNetworkPrinter(printerName, currentFileContent);

						results.Add(new { id, success = true, message = "FG Roll sticker printed successfully." });
						successCount++;
					}
					catch (Exception ex)
					{
						results.Add(new { id, success = false, message = $"Error printing FG Roll sticker: {ex.Message}" });
						errorCount++;
					}
				}

				return Ok(new { 
					message = $"Bulk print completed. Success: {successCount}, Errors: {errorCount}", 
					results,
					success = errorCount == 0
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error printing FG Roll stickers: {ex.Message}");
			}
		}

		private void PrintToNetworkPrinter(string printerIp, string content)
		{
			try
			{
				var printerAddress = IPAddress.Parse(printerIp);
				var printerPort = 9100;

				// Check if printer is reachable
				Ping ping = new Ping();
				PingReply reply = ping.Send(printerAddress, 1000);

				if (reply.Status != IPStatus.Success)
				{
					throw new Exception($"Printer at {printerIp} is not reachable.");
				}

				// Connect and send print data
				using (var client = new TcpClient())
				{
					client.Connect(printerAddress, printerPort);
					byte[] prnData = Encoding.ASCII.GetBytes(content);

					using (var stream = client.GetStream())
					{
						stream.Write(prnData, 0, prnData.Length);
						stream.Flush();
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to print: {ex.Message}");
			}
		}

		/// <summary>
		/// Sends raw print data to a USB printer using the Windows API
		/// </summary>
		/// <param name="printerName">The name of the USB printer</param>
		/// <param name="content">The raw content to print</param>
		private void PrintToUSB(string printerName, string content)
		{
			try
			{
				// Call the Windows API to send raw data to the printer
				var result = RawPrinterHelper.SendStringToPrinter(printerName, content);
				if (!result)
				{
					throw new Exception($"Failed to print to USB printer: {printerName}");
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Failed to print to USB printer {printerName}: {ex.Message}");
			}
		}

		// POST api/productionallotment
		[HttpPost]
		public async Task<IActionResult> CreateProductionAllotment([FromBody] CreateProductionAllotmentRequest request)
		{
			try
			{
				// Validate request
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				if (request.MachineAllocations == null || !request.MachineAllocations.Any())
				{
					return BadRequest("At least one machine allocation is required.");
				}

				using var transaction = await _context.Database.BeginTransactionAsync();

				// Generate the next serial number
				var nextSerialNumber = await GenerateNextSerialNumber();

				// Check if allotment ID already exists (optional duplicate check)
				if (await _context.ProductionAllotments.AnyAsync(pa => pa.AllotmentId == request.AllotmentId))
				{
					return Conflict($"Allotment ID {request.AllotmentId} already exists.");
				}

				var finalNewLotQuantity = request.ActualQuantity;

				if (request.IsSplitLotCreation && request.LotAdjustments != null && request.LotAdjustments.Any())
				{
					var salesOrder = await _context.SalesOrdersWeb
						.Include(so => so.Items)
						.FirstOrDefaultAsync(so => so.Id == request.SalesOrderId);

					if (salesOrder == null)
					{
						return NotFound($"Sales order {request.SalesOrderId} not found.");
					}

					var salesOrderItem = salesOrder.Items.FirstOrDefault(item => item.Id == request.SalesOrderItemId);
					if (salesOrderItem == null)
					{
						return NotFound($"Sales order item {request.SalesOrderItemId} not found in sales order {request.SalesOrderId}.");
					}

					var existingLots = await _context.ProductionAllotments
						.Include(pa => pa.MachineAllocations)
						.Where(pa => pa.SalesOrderId == request.SalesOrderId && pa.SalesOrderItemId == request.SalesOrderItemId)
						.ToListAsync();

					if (!existingLots.Any())
					{
						return BadRequest("No existing lots were found for this sales order item.");
					}

					var allotmentIds = existingLots.Select(lot => lot.AllotmentId).ToList();
					var readyWeightByAllotment = await _context.RollConfirmations
						.Where(rc => allotmentIds.Contains(rc.AllotId))
						.GroupBy(rc => rc.AllotId)
						.Select(group => new
						{
							AllotmentId = group.Key,
							ReadyNetWeight = group.Sum(rc => rc.NetWeight ?? 0m)
						})
						.ToDictionaryAsync(item => item.AllotmentId, item => Math.Round(item.ReadyNetWeight, 3));

					foreach (var adjustment in request.LotAdjustments)
					{
						var existingLot = existingLots.FirstOrDefault(lot => lot.Id == adjustment.LotId);
						if (existingLot == null)
						{
							return BadRequest($"Lot {adjustment.LotId} does not belong to sales order item {request.SalesOrderItemId}.");
						}

						var readyNetWeight = readyWeightByAllotment.GetValueOrDefault(existingLot.AllotmentId, 0m);
						decimal finalQuantity;

						if (adjustment.MarkAsComplete)
						{
							if (readyNetWeight <= 0)
							{
								return BadRequest($"Cannot complete lot {existingLot.AllotmentId}: no ready net weight has been produced yet.");
							}

							finalQuantity = readyNetWeight;
							existingLot.ProductionStatus = 2;
						}
						else
						{
							finalQuantity = Math.Round(adjustment.FinalQuantity, 3);

							if (finalQuantity <= 0)
							{
								return BadRequest($"Lot {existingLot.AllotmentId} must keep a planned quantity greater than zero.");
							}

							if (finalQuantity > existingLot.ActualQuantity)
							{
								return BadRequest($"Lot {existingLot.AllotmentId} cannot keep more than its current planned quantity ({existingLot.ActualQuantity:F3} kg).");
							}

							existingLot.ProductionStatus = 3;
						}

						existingLot.ActualQuantity = finalQuantity;
						RecalculateMachineAllocationsForQuantity(existingLot, finalQuantity);
					}

					var totalExistingPlannedQuantity = existingLots.Sum(lot => lot.ActualQuantity);
					finalNewLotQuantity = Math.Round(salesOrderItem.Qty - totalExistingPlannedQuantity, 3);

					if (finalNewLotQuantity <= 0)
					{
						return BadRequest(new
						{
							Message = "No remaining quantity is available for a new lot after the selected lot adjustments.",
							SalesOrderItemQuantity = salesOrderItem.Qty,
							ExistingPlannedQuantity = totalExistingPlannedQuantity
						});
					}

					if (request.ActualQuantity > 0 && Math.Abs(request.ActualQuantity - finalNewLotQuantity) > 0.01m)
					{
						return BadRequest(new
						{
							Message = $"New lot quantity mismatch. Expected {finalNewLotQuantity:F3} kg but received {request.ActualQuantity:F3} kg.",
							ExpectedQuantity = finalNewLotQuantity,
							ReceivedQuantity = request.ActualQuantity
						});
					}
				}

				// Create production allotment using the allotmentId from the request
				var productionAllotment = new ProductionAllotment
				{
					AllotmentId = request.AllotmentId, // Use the ID from frontend
					VoucherNumber = request.VoucherNumber,
					ItemName = request.ItemName,
					SalesOrderId = request.SalesOrderId,
					SalesOrderItemId = request.SalesOrderItemId,
					ActualQuantity = finalNewLotQuantity,
					YarnCount = request.YarnCount,
					Diameter = request.Diameter,
					Gauge = request.Gauge,
					FabricType = request.FabricType,
					SlitLine = request.SlitLine,
					StitchLength = request.StitchLength,
					Efficiency = request.Efficiency,
					Composition = request.Composition,
					YarnLotNo = request.YarnLotNo,
					Counter = request.Counter,
					ColourCode = request.ColourCode,
					ReqGreyGsm = request.ReqGreyGsm,
					ReqGreyWidth = request.ReqGreyWidth,
					ReqFinishGsm = request.ReqFinishGsm,
					ReqFinishWidth = request.ReqFinishWidth,
					YarnPartyName = request.YarnPartyName, // New field for yarn party name
					PolybagColor = request.PolybagColor, // New field for polybag color
					PartyName = request.PartyName,
					OtherReference = request.OtherReference,
					TubeWeight = request.TubeWeight,
					ShrinkRapWeight = request.ShrinkRapWeight,
					TotalWeight = request.TotalWeight,
					TapeColor = request.TapeColor,
					SerialNo = nextSerialNumber, // Assign the generated serial number
					ProductionStatus = request.IsSplitLotCreation ? 3 : 0,
					TotalProductionTime = request.MachineAllocations.Max(ma => ma.EstimatedProductionTime),
					MachineAllocations = request.MachineAllocations.Select(ma => new MachineAllocation
					{
						MachineName = ma.MachineName,
						MachineId = ma.MachineId,
						NumberOfNeedles = ma.NumberOfNeedles,
						Feeders = ma.Feeders,
						RPM = ma.RPM,
						RollPerKg = ma.RollPerKg,
						TotalLoadWeight = ma.TotalLoadWeight,
						TotalRolls = ma.TotalRolls,
						RollBreakdown = System.Text.Json.JsonSerializer.Serialize(ma.RollBreakdown),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList()
				};

				// Save to database
				_context.ProductionAllotments.Add(productionAllotment);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return Ok(new
				{
					Success = true,
					AllotmentId = request.AllotmentId,
					ProductionAllotmentId = productionAllotment.Id,
					SerialNo = nextSerialNumber,
					ActualQuantity = finalNewLotQuantity
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating production allotment");
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// Helper method to generate the next serial number
		private async Task<string> GenerateNextSerialNumber()
		{
			// Get the maximum serial number from existing production allotments and add 1
			var maxSerialNumber = 0;
			var existingAllotments = await _context.ProductionAllotments
				.Select(pa => pa.SerialNo)
				.ToListAsync();

			if (existingAllotments.Any())
			{
				foreach (var serialNo in existingAllotments)
				{
					// Parse the serial number (format: "0001", "0002", etc.)
					if (int.TryParse(serialNo, out int serial))
					{
						if (serial > maxSerialNumber)
							maxSerialNumber = serial;
					}
				}
			}

			var nextNumber = maxSerialNumber + 1;

			// Format as 4-digit zero-padded string
			return nextNumber.ToString("D4");
		}

		// PUT api/productionallotment/machine-allocations/{allotmentId}
		[HttpPut("machine-allocations/{allotmentId}")]
		public async Task<IActionResult> UpdateMachineAllocations(string allotmentId, [FromBody] AvyyanBackend.DTOs.ProAllotDto.UpdateMachineAllocationsRequest request)
		{
			try
			{
				// Validate request
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				// Find the production allotment by AllotmentId
				var productionAllotment = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.FirstOrDefaultAsync(pa => pa.AllotmentId == allotmentId);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with ID {allotmentId} not found.");
				}

				// Check if the total allocated rolls matches the actual quantity
				// var totalAllocatedRolls = request.MachineAllocations.Sum(ma => ma.TotalRolls);
				// if (Math.Abs(totalAllocatedRolls - productionAllotment.ActualQuantity) > 0.01m)
				// {
				// 	return BadRequest($"Total allocated rolls ({totalAllocatedRolls}) must exactly match actual quantity ({productionAllotment.ActualQuantity})");
				// }

				// Update machine allocations
				// First, remove existing allocations that are not in the request
				var existingMachineIds = request.MachineAllocations.Where(ma => ma.Id.HasValue).Select(ma => ma.Id.Value).ToList();
				var allocationsToRemove = productionAllotment.MachineAllocations
					.Where(ma => !existingMachineIds.Contains(ma.Id))
					.ToList();

				foreach (var allocation in allocationsToRemove)
				{
					_context.MachineAllocations.Remove(allocation);
				}

				// Update existing allocations or add new ones
				foreach (var requestAllocation in request.MachineAllocations)
				{
					MachineAllocation dbAllocation;

					if (requestAllocation.Id.HasValue)
					{
						// Update existing allocation
						dbAllocation = productionAllotment.MachineAllocations
							.FirstOrDefault(ma => ma.Id == requestAllocation.Id.Value);

						if (dbAllocation == null)
						{
							return BadRequest($"Machine allocation with ID {requestAllocation.Id.Value} not found.");
						}
					}
					else
					{
						// Add new allocation
						dbAllocation = new MachineAllocation
						{
							ProductionAllotmentId = productionAllotment.Id
						};
						productionAllotment.MachineAllocations.Add(dbAllocation);
						_context.MachineAllocations.Add(dbAllocation);
					}

					// Update allocation properties
					dbAllocation.MachineName = requestAllocation.MachineName;
					dbAllocation.MachineId = requestAllocation.MachineId;
					dbAllocation.NumberOfNeedles = requestAllocation.NumberOfNeedles;
					dbAllocation.Feeders = requestAllocation.Feeders;
					dbAllocation.RPM = requestAllocation.RPM;
					dbAllocation.RollPerKg = requestAllocation.RollPerKg;
					dbAllocation.TotalLoadWeight = requestAllocation.TotalLoadWeight;
					dbAllocation.TotalRolls = requestAllocation.TotalRolls;
					dbAllocation.RollBreakdown = System.Text.Json.JsonSerializer.Serialize(requestAllocation.RollBreakdown);
					dbAllocation.EstimatedProductionTime = requestAllocation.EstimatedProductionTime;
				}

				// Update the total production time
				productionAllotment.TotalProductionTime = productionAllotment.MachineAllocations.Sum(ma => ma.EstimatedProductionTime);

				// Save changes
				await _context.SaveChangesAsync();

				// Return updated production allotment
				var responseDto = new ProductionAllotmentResponseDto
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
					OtherReference = productionAllotment.OtherReference,
					TubeWeight = productionAllotment.TubeWeight,
					ShrinkRapWeight = productionAllotment.ShrinkRapWeight,
					TotalWeight = productionAllotment.TotalWeight,
					TapeColor = productionAllotment.TapeColor,
					SerialNo = productionAllotment.SerialNo,
					ProductionStatus = productionAllotment.ProductionStatus,
					IsOnHold = productionAllotment.ProductionStatus == 1,
					IsSuspended = productionAllotment.ProductionStatus == 2,
					MachineAllocations = productionAllotment.MachineAllocations.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList()
				};

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating machine allocations for production allotment: {AllotmentId}", allotmentId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT api/productionallotment/{id}/hold - Toggle hold status
		[HttpPut("{id}/hold")]
		public async Task<ActionResult<ProductionAllotmentResponseDto>> ToggleHold(int id)
		{
			try
			{
				var productionAllotment = await _context.ProductionAllotments.FindAsync(id);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with ID {id} not found.");
				}

				// Toggle the hold status - if currently on hold (1), set to normal (0); if normal (0) or suspended (2), set to on hold (1)
				productionAllotment.ProductionStatus = productionAllotment.ProductionStatus == 1 ? 0 : 1;

				await _context.SaveChangesAsync();

				// Return the updated production allotment
				var responseDto = new ProductionAllotmentResponseDto
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
					TotalProductionTime = productionAllotment.MachineAllocations?.Sum(ma => ma.EstimatedProductionTime) ?? 0,
					CreatedDate = productionAllotment.CreatedDate,
					YarnLotNo = productionAllotment.YarnLotNo,
					Counter = productionAllotment.Counter,
					ColourCode = productionAllotment.ColourCode,
					ReqGreyGsm = productionAllotment.ReqGreyGsm,
					ReqGreyWidth = productionAllotment.ReqGreyWidth,
					ReqFinishGsm = productionAllotment.ReqFinishGsm,
					ReqFinishWidth = productionAllotment.ReqFinishWidth,
					YarnPartyName = productionAllotment.YarnPartyName,
					PolybagColor = productionAllotment.PolybagColor,
					PartyName = productionAllotment.PartyName,
					OtherReference = productionAllotment.OtherReference,
					TubeWeight = productionAllotment.TubeWeight,
					ShrinkRapWeight = productionAllotment.ShrinkRapWeight,
					TotalWeight = productionAllotment.TotalWeight,
					TapeColor = productionAllotment.TapeColor,
					SerialNo = productionAllotment.SerialNo,
					ProductionStatus = productionAllotment.ProductionStatus,
					IsOnHold = productionAllotment.ProductionStatus == 1,
					IsSuspended = productionAllotment.ProductionStatus == 2,
					MachineAllocations = productionAllotment.MachineAllocations?.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList() ?? new List<MachineAllocationResponseDto>()
				};

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error toggling hold status for production allotment: {Id}", id);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT api/productionallotment/{id}/suspend - Suspend production planning
		[HttpPut("{id}/suspend")]
		public async Task<ActionResult<ProductionAllotmentResponseDto>> SuspendPlanning(int id)
		{
			try
			{
				var productionAllotment = await _context.ProductionAllotments.FindAsync(id);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with ID {id} not found.");
				}

				// Set the suspended status to 2
				productionAllotment.ProductionStatus = 2;

				await _context.SaveChangesAsync();

				// Return the updated production allotment
				var responseDto = new ProductionAllotmentResponseDto
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
					TotalProductionTime = productionAllotment.MachineAllocations?.Sum(ma => ma.EstimatedProductionTime) ?? 0,
					CreatedDate = productionAllotment.CreatedDate,
					YarnLotNo = productionAllotment.YarnLotNo,
					Counter = productionAllotment.Counter,
					ColourCode = productionAllotment.ColourCode,
					ReqGreyGsm = productionAllotment.ReqGreyGsm,
					ReqGreyWidth = productionAllotment.ReqGreyWidth,
					ReqFinishGsm = productionAllotment.ReqFinishGsm,
					ReqFinishWidth = productionAllotment.ReqFinishWidth,
					YarnPartyName = productionAllotment.YarnPartyName,
					PolybagColor = productionAllotment.PolybagColor,
					PartyName = productionAllotment.PartyName,
					OtherReference = productionAllotment.OtherReference,
					TubeWeight = productionAllotment.TubeWeight,
					ShrinkRapWeight = productionAllotment.ShrinkRapWeight,
					TotalWeight = productionAllotment.TotalWeight,
					TapeColor = productionAllotment.TapeColor,
					SerialNo = productionAllotment.SerialNo,
					ProductionStatus = productionAllotment.ProductionStatus,
					IsOnHold = productionAllotment.ProductionStatus == 1,
					IsSuspended = productionAllotment.ProductionStatus == 2,
					MachineAllocations = productionAllotment.MachineAllocations?.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList() ?? new List<MachineAllocationResponseDto>()
				};

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error suspending production planning for production allotment: {Id}", id);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT api/productionallotment/{id}/status - Update production status
		[HttpPut("{id}/status")]
		public async Task<ActionResult<ProductionAllotmentResponseDto>> UpdateProductionStatus(int id, [FromBody] UpdateStatusRequest request)
		{
			try
			{
				var productionAllotment = await _context.ProductionAllotments.FindAsync(id);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with ID {id} not found.");
				}

				// Validate the status value (0 = normal, 1 = on hold, 2 = suspended, 3 = partially completed)
				if (request.Status < 0 || request.Status > 3)
				{
					return BadRequest("Invalid production status value.");
				}

				// Update the production status
				productionAllotment.ProductionStatus = request.Status;

				await _context.SaveChangesAsync();

				// Return the updated production allotment
				var responseDto = new ProductionAllotmentResponseDto
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
					TotalProductionTime = productionAllotment.MachineAllocations?.Sum(ma => ma.EstimatedProductionTime) ?? 0,
					CreatedDate = productionAllotment.CreatedDate,
					YarnLotNo = productionAllotment.YarnLotNo,
					Counter = productionAllotment.Counter,
					ColourCode = productionAllotment.ColourCode,
					ReqGreyGsm = productionAllotment.ReqGreyGsm,
					ReqGreyWidth = productionAllotment.ReqGreyWidth,
					ReqFinishGsm = productionAllotment.ReqFinishGsm,
					ReqFinishWidth = productionAllotment.ReqFinishWidth,
					YarnPartyName = productionAllotment.YarnPartyName,
					PolybagColor = productionAllotment.PolybagColor,
					PartyName = productionAllotment.PartyName,
					OtherReference = productionAllotment.OtherReference,
					TubeWeight = productionAllotment.TubeWeight,
					ShrinkRapWeight = productionAllotment.ShrinkRapWeight,
					TotalWeight = productionAllotment.TotalWeight,
					TapeColor = productionAllotment.TapeColor,
					SerialNo = productionAllotment.SerialNo,
					ProductionStatus = productionAllotment.ProductionStatus,
					IsOnHold = productionAllotment.ProductionStatus == 1,
					IsSuspended = productionAllotment.ProductionStatus == 2,
					MachineAllocations = productionAllotment.MachineAllocations?.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList() ?? new List<MachineAllocationResponseDto>()
				};

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating production status for production allotment: {Id}", id);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT api/productionallotment/{id}/restart - Restart production from suspended status
		[HttpPut("{id}/restart")]
		public async Task<ActionResult<ProductionAllotmentResponseDto>> RestartProduction(int id)
		{
			try
			{
				var productionAllotment = await _context.ProductionAllotments.FindAsync(id);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with ID {id} not found.");
				}

				// Only allow restart if currently suspended (status 2)
				if (productionAllotment.ProductionStatus != 2)
				{
					return BadRequest("Production can only be restarted from suspended status.");
				}

				// Set the status back to normal (0) to restart production
				productionAllotment.ProductionStatus = 0;

				await _context.SaveChangesAsync();

				// Return the updated production allotment
				var responseDto = new ProductionAllotmentResponseDto
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
					TotalProductionTime = productionAllotment.MachineAllocations?.Sum(ma => ma.EstimatedProductionTime) ?? 0,
					CreatedDate = productionAllotment.CreatedDate,
					YarnLotNo = productionAllotment.YarnLotNo,
					Counter = productionAllotment.Counter,
					ColourCode = productionAllotment.ColourCode,
					ReqGreyGsm = productionAllotment.ReqGreyGsm,
					ReqGreyWidth = productionAllotment.ReqGreyWidth,
					ReqFinishGsm = productionAllotment.ReqFinishGsm,
					ReqFinishWidth = productionAllotment.ReqFinishWidth,
					YarnPartyName = productionAllotment.YarnPartyName,
					PolybagColor = productionAllotment.PolybagColor,
					PartyName = productionAllotment.PartyName,
					OtherReference = productionAllotment.OtherReference,
					TubeWeight = productionAllotment.TubeWeight,
					ShrinkRapWeight = productionAllotment.ShrinkRapWeight,
					TotalWeight = productionAllotment.TotalWeight,
					TapeColor = productionAllotment.TapeColor,
					SerialNo = productionAllotment.SerialNo,
					ProductionStatus = productionAllotment.ProductionStatus,
					IsOnHold = productionAllotment.ProductionStatus == 1,
					IsSuspended = productionAllotment.ProductionStatus == 2,
					MachineAllocations = productionAllotment.MachineAllocations?.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList() ?? new List<MachineAllocationResponseDto>()
				};

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error restarting production for production allotment: {Id}", id);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/productionallotment/{allotmentId}/status - Check if stickers have been generated or roll confirmations exist
		[HttpGet("{allotmentId}/status")]
		public async Task<ActionResult<object>> CheckAllotmentStatus(string allotmentId)
		{
			try
			{
				// Get ProductionAllotment ID based on the AllotmentId
				var productionAllotment = await _context.ProductionAllotments
					.FirstOrDefaultAsync(pa => pa.AllotmentId == allotmentId);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with ID {allotmentId} not found.");
				}

				// Get MachineAllocations based on ProductionAllotment ID (FK)
				var machineAllocations = await _context.MachineAllocations
					.Where(ma => ma.ProductionAllotmentId == productionAllotment.Id)
					.Select(ma => ma.Id)
					.ToListAsync();

				// Check if any RollAssignments exist based on MachineAllocation IDs (FK)
				bool hasRollAssignment = false;
				if (machineAllocations.Any())
				{
					hasRollAssignment = await _context.RollAssignments
						.AnyAsync(ra => machineAllocations.Contains(ra.MachineAllocationId));
				}

				// For sticker generation, we'll consider it generated if roll assignments exist
				bool hasStickersGenerated = hasRollAssignment;

				// Also check if any roll confirmations exist for this allotment
				var hasRollConfirmation = await _context.RollConfirmations.AnyAsync(rc => rc.AllotId == allotmentId);

				if (hasRollConfirmation)
				{
					hasStickersGenerated = false;
					hasRollAssignment = false;
                }
                var statusResponse = new
				{
					HasRollConfirmation = hasRollConfirmation,
					HasStickersGenerated = hasStickersGenerated,
					HasRollAssignment = hasRollAssignment
				};

				return Ok(statusResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error checking status for production allotment: {AllotmentId}", allotmentId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/productionallotment/sales-order/{salesOrderId}/items/{salesOrderItemId}/roll-confirmation-summary - Get roll confirmation summary for a sales order item
		[HttpGet("sales-order/{salesOrderId}/items/{salesOrderItemId}/roll-confirmation-summary")]
		public async Task<ActionResult<object>> GetRollConfirmationSummaryForSalesOrderItem(int salesOrderId, int salesOrderItemId)
		{
			try
			{
				// Get all production allotments for the sales order item
				var productionAllotments = await _context.ProductionAllotments
					.Where(pa => pa.SalesOrderId == salesOrderId && pa.SalesOrderItemId == salesOrderItemId)
					.Select(pa => pa.AllotmentId)
					.ToListAsync();

				if (!productionAllotments.Any())
				{
					return Ok(new { TotalLots = 0, TotalRollConfirmations = 0, TotalNetWeight = 0.0m });
				}

				// Get all roll confirmations for these lots
				var rollConfirmations = await _context.RollConfirmations
					.Where(rc => productionAllotments.Contains(rc.AllotId))
					.ToListAsync();

				var totalNetWeight = rollConfirmations.Sum(rc => rc.NetWeight) ?? 0.0m;

				var summary = new
				{
					TotalLots = productionAllotments.Count,
					TotalRollConfirmations = rollConfirmations.Count,
					TotalNetWeight = totalNetWeight
				};

				return Ok(summary);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching roll confirmation summary for sales order item: {SalesOrderId}, {SalesOrderItemId}", salesOrderId, salesOrderItemId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/productionallotment/sales-order/{salesOrderId}/lots - Get all lots for a sales order (without requiring itemId)
		[HttpGet("sales-order/{salesOrderId}/lots")]
		public async Task<ActionResult<IEnumerable<ProductionAllotmentResponseDto>>> GetLotsForSalesOrder(int salesOrderId)
		{
			try
			{
				var productionAllotments = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.Where(pa => pa.SalesOrderId == salesOrderId)
					.ToListAsync();

				var responseDtos = productionAllotments.Select(pa => new ProductionAllotmentResponseDto
				{
					Id = pa.Id,
					AllotmentId = pa.AllotmentId,
					VoucherNumber = pa.VoucherNumber,
					ItemName = pa.ItemName,
					SalesOrderId = pa.SalesOrderId,
					SalesOrderItemId = pa.SalesOrderItemId,
					ActualQuantity = pa.ActualQuantity,
					YarnCount = pa.YarnCount,
					Diameter = pa.Diameter,
					Gauge = pa.Gauge,
					FabricType = pa.FabricType,
					SlitLine = pa.SlitLine,
					StitchLength = pa.StitchLength,
					Efficiency = pa.Efficiency,
					Composition = pa.Composition,
					TotalProductionTime = pa.TotalProductionTime,
					CreatedDate = pa.CreatedDate,
					YarnLotNo = pa.YarnLotNo,
					Counter = pa.Counter,
					ColourCode = pa.ColourCode,
					ReqGreyGsm = pa.ReqGreyGsm,
					ReqGreyWidth = pa.ReqGreyWidth,
					ReqFinishGsm = pa.ReqFinishGsm,
					ReqFinishWidth = pa.ReqFinishWidth,
					YarnPartyName = pa.YarnPartyName,
					PolybagColor = pa.PolybagColor,
					PartyName = pa.PartyName,
					OtherReference = pa.OtherReference,
					TubeWeight = pa.TubeWeight,
					ShrinkRapWeight = pa.ShrinkRapWeight,
					TotalWeight = pa.TotalWeight,
					TapeColor = pa.TapeColor,
					SerialNo = pa.SerialNo,
					ProductionStatus = pa.ProductionStatus,
					IsOnHold = pa.ProductionStatus == 1,
					IsSuspended = pa.ProductionStatus == 2,
					MachineAllocations = pa.MachineAllocations.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList()
				}).ToList();

				return Ok(responseDtos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching lots for sales order: {SalesOrderId}", salesOrderId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/productionallotment/sales-order/{salesOrderId}/items/{salesOrderItemId}/lots - Get lots for a sales order item
		[HttpGet("sales-order/{salesOrderId}/items/{salesOrderItemId}/lots")]
		public async Task<ActionResult<IEnumerable<ProductionAllotmentResponseDto>>> GetLotsForSalesOrderItem(int salesOrderId, int salesOrderItemId)
		{
			try
			{
				var productionAllotments = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.Where(pa => pa.SalesOrderId == salesOrderId && pa.SalesOrderItemId == salesOrderItemId)
					.ToListAsync();

				var responseDtos = productionAllotments.Select(pa => new ProductionAllotmentResponseDto
				{
					Id = pa.Id,
					AllotmentId = pa.AllotmentId,
					VoucherNumber = pa.VoucherNumber,
					ItemName = pa.ItemName,
					SalesOrderId = pa.SalesOrderId,
					SalesOrderItemId = pa.SalesOrderItemId,
					ActualQuantity = pa.ActualQuantity,
					YarnCount = pa.YarnCount,
					Diameter = pa.Diameter,
					Gauge = pa.Gauge,
					FabricType = pa.FabricType,
					SlitLine = pa.SlitLine,
					StitchLength = pa.StitchLength,
					Efficiency = pa.Efficiency,
					Composition = pa.Composition,
					TotalProductionTime = pa.TotalProductionTime,
					CreatedDate = pa.CreatedDate,
					YarnLotNo = pa.YarnLotNo,
					Counter = pa.Counter,
					ColourCode = pa.ColourCode,
					ReqGreyGsm = pa.ReqGreyGsm,
					ReqGreyWidth = pa.ReqGreyWidth,
					ReqFinishGsm = pa.ReqFinishGsm,
					ReqFinishWidth = pa.ReqFinishWidth,
					YarnPartyName = pa.YarnPartyName,
					PolybagColor = pa.PolybagColor,
					PartyName = pa.PartyName,
					OtherReference = pa.OtherReference,
					TubeWeight = pa.TubeWeight,
					ShrinkRapWeight = pa.ShrinkRapWeight,
					TotalWeight = pa.TotalWeight,
					TapeColor = pa.TapeColor,
					SerialNo = pa.SerialNo,
					ProductionStatus = pa.ProductionStatus,
					IsOnHold = pa.ProductionStatus == 1,
					IsSuspended = pa.ProductionStatus == 2,
					MachineAllocations = pa.MachineAllocations.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList()
				}).ToList();

				return Ok(responseDtos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching lots for sales order item: {SalesOrderId}, {SalesOrderItemId}", salesOrderId, salesOrderItemId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/productionallotment/sales-order/{salesOrderId}/allocation-summary
		// Returns allocation summary for all items in a sales order, including created roll net weights
		[HttpGet("sales-order/{salesOrderId}/allocation-summary")]
		public async Task<ActionResult<object>> GetAllocationSummaryForSalesOrder(int salesOrderId)
		{
			try
			{
				// Get all production allotments for this sales order
				var productionAllotments = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.Where(pa => pa.SalesOrderId == salesOrderId)
					.ToListAsync();

				// Get all allotment IDs
				var allotmentIds = productionAllotments.Select(pa => pa.AllotmentId).ToList();

				// Get all roll confirmations for these lots
				var rollConfirmations = await _context.RollConfirmations
					.Where(rc => allotmentIds.Contains(rc.AllotId))
					.ToListAsync();

				// Group by SalesOrderItemId
				var itemGroups = productionAllotments
					.GroupBy(pa => pa.SalesOrderItemId)
					.Select(group =>
					{
						var lots = group.Select(pa =>
						{
							var lotRollConfirmations = rollConfirmations
								.Where(rc => rc.AllotId == pa.AllotmentId)
								.ToList();
							var createdRollNetWeight = lotRollConfirmations.Sum(rc => rc.NetWeight ?? 0m);
							var createdRollCount = lotRollConfirmations.Count;

							return new
							{
								Id = pa.Id,
								AllotmentId = pa.AllotmentId,
								ActualQuantity = pa.ActualQuantity,
								ProductionStatus = pa.ProductionStatus,
								IsOnHold = pa.ProductionStatus == 1,
								IsSuspended = pa.ProductionStatus == 2,
								CreatedRollNetWeight = createdRollNetWeight,
								CreatedRollCount = createdRollCount,
								CreatedDate = pa.CreatedDate,
								YarnCount = pa.YarnCount,
								FabricType = pa.FabricType,
								SlitLine = pa.SlitLine,
								StitchLength = pa.StitchLength,
								Composition = pa.Composition,
								Diameter = pa.Diameter,
								Gauge = pa.Gauge,
								MachineAllocations = pa.MachineAllocations.Select(ma => new MachineAllocationResponseDto
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
									RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
										? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
										: new DTOs.ProAllotDto.RollBreakdown(),
									EstimatedProductionTime = ma.EstimatedProductionTime
								}).ToList()
							};
						}).ToList();

						return new
						{
							SalesOrderItemId = group.Key,
							TotalAllottedQuantity = lots.Sum(l => l.ActualQuantity),
							TotalCreatedRollNetWeight = lots.Sum(l => l.CreatedRollNetWeight),
							TotalCreatedRollCount = lots.Sum(l => l.CreatedRollCount),
							LotCount = lots.Count,
							Lots = lots
						};
					}).ToList();

				return Ok(new
				{
					SalesOrderId = salesOrderId,
					Items = itemGroups
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching allocation summary for sales order: {SalesOrderId}", salesOrderId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// DTO for update quantity request
		public class UpdateLotQuantityRequest
		{
			public decimal NewQuantity { get; set; }
		}

		// PUT api/productionallotment/{id}/update-quantity
		// Updates a lot's quantity with validation (min = created roll net weight)
		// Recalculates machine allocations based on per-roll-kg
		[HttpPut("{id}/update-quantity")]
		public async Task<ActionResult<object>> UpdateLotQuantity(int id, [FromBody] UpdateLotQuantityRequest request)
		{
			try
			{
				var productionAllotment = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.FirstOrDefaultAsync(pa => pa.Id == id);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with ID {id} not found.");
				}

				// Get created roll net weight for validation
				var rollConfirmations = await _context.RollConfirmations
					.Where(rc => rc.AllotId == productionAllotment.AllotmentId)
					.ToListAsync();

				var createdRollNetWeight = rollConfirmations.Sum(rc => rc.NetWeight ?? 0m);

				// Validation: Cannot reduce below created roll net weight
				if (request.NewQuantity < createdRollNetWeight)
				{
					return BadRequest(new
					{
						Message = $"Cannot reduce quantity below created roll net weight ({createdRollNetWeight:F3} kg). Total net weight of {rollConfirmations.Count} created rolls is {createdRollNetWeight:F3} kg.",
						CreatedRollNetWeight = createdRollNetWeight,
						CreatedRollCount = rollConfirmations.Count
					});
				}

				// Validation: Quantity must be positive
				if (request.NewQuantity <= 0)
				{
					return BadRequest("Quantity must be greater than zero.");
				}

				var oldQuantity = productionAllotment.ActualQuantity;
				productionAllotment.ActualQuantity = request.NewQuantity;

				RecalculateMachineAllocationsForQuantity(productionAllotment, request.NewQuantity);

				await _context.SaveChangesAsync();

				// Return updated data
				return Ok(new
				{
					Success = true,
					Message = $"Lot quantity updated from {oldQuantity:F3} to {request.NewQuantity:F3} kg",
					Id = productionAllotment.Id,
					AllotmentId = productionAllotment.AllotmentId,
					OldQuantity = oldQuantity,
					NewQuantity = request.NewQuantity,
					CreatedRollNetWeight = createdRollNetWeight,
					MachineAllocations = productionAllotment.MachineAllocations?.Select(ma => new MachineAllocationResponseDto
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
						RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
							? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
							: new DTOs.ProAllotDto.RollBreakdown(),
						EstimatedProductionTime = ma.EstimatedProductionTime
					}).ToList()
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating lot quantity for production allotment: {Id}", id);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// PUT api/productionallotment/{id}/complete-lot
		// Completes a lot: sets quantity to total net weight of created rolls, returns remaining quantity
		[HttpPut("{id}/complete-lot")]
		public async Task<ActionResult<object>> CompleteLot(int id)
		{
			try
			{
				var productionAllotment = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.FirstOrDefaultAsync(pa => pa.Id == id);

				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with ID {id} not found.");
				}

				// Get created roll net weight
				var rollConfirmations = await _context.RollConfirmations
					.Where(rc => rc.AllotId == productionAllotment.AllotmentId)
					.ToListAsync();

				var createdRollNetWeight = rollConfirmations.Sum(rc => rc.NetWeight ?? 0m);

				if (createdRollNetWeight <= 0)
				{
					return BadRequest("Cannot complete lot: no rolls have been created yet.");
				}

				var oldQuantity = productionAllotment.ActualQuantity;
				var remainingQuantity = oldQuantity - createdRollNetWeight;

				// Set the lot quantity to the created roll net weight
				productionAllotment.ActualQuantity = createdRollNetWeight;

				// Mark as completed (suspended status = 2 means no more production needed)
				productionAllotment.ProductionStatus = 2;

				RecalculateMachineAllocationsForQuantity(productionAllotment, createdRollNetWeight);

				await _context.SaveChangesAsync();

				return Ok(new
				{
					Success = true,
					Message = $"Lot completed. Quantity set to {createdRollNetWeight:F3} kg (created roll weight). Remaining: {Math.Max(remainingQuantity, 0):F3} kg",
					Id = productionAllotment.Id,
					AllotmentId = productionAllotment.AllotmentId,
					SalesOrderId = productionAllotment.SalesOrderId,
					SalesOrderItemId = productionAllotment.SalesOrderItemId,
					OldQuantity = oldQuantity,
					CompletedQuantity = createdRollNetWeight,
					RemainingQuantity = Math.Max(remainingQuantity, 0),
					CreatedRollCount = rollConfirmations.Count,
					ItemName = productionAllotment.ItemName,
					VoucherNumber = productionAllotment.VoucherNumber
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error completing lot for production allotment: {Id}", id);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		private void RecalculateMachineAllocationsForQuantity(ProductionAllotment productionAllotment, decimal targetQuantity)
		{
			if (productionAllotment.MachineAllocations == null || !productionAllotment.MachineAllocations.Any())
			{
				productionAllotment.TotalProductionTime = 0;
				return;
			}

			var totalExistingLoad = productionAllotment.MachineAllocations.Sum(ma => ma.TotalLoadWeight);
			var ratio = totalExistingLoad > 0 ? targetQuantity / totalExistingLoad : 1m;

			foreach (var machineAllocation in productionAllotment.MachineAllocations)
			{
				machineAllocation.TotalLoadWeight = Math.Round(machineAllocation.TotalLoadWeight * ratio, 3);
				machineAllocation.EstimatedProductionTime = Math.Round(machineAllocation.EstimatedProductionTime * ratio, 2);

				if (machineAllocation.RollPerKg > 0)
				{
					var exactTotalRolls = machineAllocation.TotalLoadWeight / machineAllocation.RollPerKg;
					machineAllocation.TotalRolls = Math.Ceiling(exactTotalRolls);
					machineAllocation.RollBreakdown = System.Text.Json.JsonSerializer.Serialize(
						BuildRollBreakdown(exactTotalRolls, machineAllocation.RollPerKg)
					);
				}
				else
				{
					machineAllocation.TotalRolls = 0;
					machineAllocation.RollBreakdown = System.Text.Json.JsonSerializer.Serialize(
						new DTOs.ProAllotDto.RollBreakdown
						{
							WholeRolls = new List<DTOs.ProAllotDto.RollItem>(),
							FractionalRoll = null
						}
					);
				}
			}

			productionAllotment.TotalProductionTime = productionAllotment.MachineAllocations.Sum(ma => ma.EstimatedProductionTime);
		}

		private DTOs.ProAllotDto.RollBreakdown BuildRollBreakdown(decimal exactTotalRolls, decimal rollPerKg)
		{
			var wholeRolls = (int)Math.Floor(exactTotalRolls);
			var fractionalPart = exactTotalRolls - wholeRolls;
			var rollBreakdown = new DTOs.ProAllotDto.RollBreakdown
			{
				WholeRolls = new List<DTOs.ProAllotDto.RollItem>(),
				FractionalRoll = null
			};

			if (wholeRolls > 0)
			{
				rollBreakdown.WholeRolls.Add(new DTOs.ProAllotDto.RollItem
				{
					Quantity = wholeRolls,
					WeightPerRoll = rollPerKg,
					TotalWeight = Math.Round(wholeRolls * rollPerKg, 3)
				});
			}

			if (fractionalPart > 0.001m)
			{
				var fractionalWeight = Math.Round(fractionalPart * rollPerKg, 3);
				rollBreakdown.FractionalRoll = new DTOs.ProAllotDto.RollItem
				{
					Quantity = 1,
					WeightPerRoll = fractionalWeight,
					TotalWeight = fractionalWeight
				};
			}

			return rollBreakdown;
		}

	}
}

