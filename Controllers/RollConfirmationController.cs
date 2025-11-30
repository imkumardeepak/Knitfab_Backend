using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.ProductionConfirmation;
using AvyyanBackend.Models.ProductionConfirmation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Text;

namespace AvyyanBackend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RollConfirmationController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<RollConfirmationController> _logger;
		private readonly IMapper _mapper;

		public RollConfirmationController(ApplicationDbContext context, ILogger<RollConfirmationController> logger, IMapper mapper)
		{
			_context = context;
			_logger = logger;
			_mapper = mapper;
		}

		// POST api/rollconfirmation
		[HttpPost]
		public async Task<ActionResult<RollConfirmationResponseDto>> CreateRollConfirmation([FromBody] RollConfirmationRequestDto request)
		{
			try
			{
				// Validate request
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				// Check if this roll confirmation already exists
				var existingRoll = await _context.RollConfirmations
					.FirstOrDefaultAsync(r => r.AllotId == request.AllotId &&
										 r.MachineName == request.MachineName &&
										 r.RollNo == request.RollNo);

				if (existingRoll != null)
				{
					return Conflict($"Roll confirmation for Allot ID {request.AllotId}, Machine {request.MachineName}, Roll No {request.RollNo} already exists.");
				}

				// Automatically assign FG Roll Number if not provided
				// int? fgRollNo = request.FgRollNo;
				// if (!fgRollNo.HasValue)
				// {
				// 	// Get the maximum FG Roll Number for this AllotId and increment by 1
				// 	var maxFgRollNo = await _context.RollConfirmations
				// 		.Where(r => r.AllotId == request.AllotId)
				// 		.MaxAsync(r => (int?)r.FgRollNo);

				// 	fgRollNo = (maxFgRollNo ?? 0) + 1;
				// }

				// Create roll confirmation entity
				var rollConfirmation = new RollConfirmation
				{
					AllotId = request.AllotId,
					MachineName = request.MachineName,
					RollPerKg = request.RollPerKg,
					GreyGsm = request.GreyGsm,
					GreyWidth = request.GreyWidth,
					BlendPercent = request.BlendPercent,
					Cotton = request.Cotton,
					Polyester = request.Polyester,
					Spandex = request.Spandex,
					RollNo = request.RollNo,
					FgRollNo = null, // Assign the FG Roll Number
					CreatedDate = request.CreatedDate
				};

				// Add to database
				_context.RollConfirmations.Add(rollConfirmation);
				await _context.SaveChangesAsync();

				// Create response DTO
				var responseDto = new RollConfirmationResponseDto
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
					FgRollNo = rollConfirmation.FgRollNo, // Include in response
					CreatedDate = rollConfirmation.CreatedDate
				};

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating roll confirmation");
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/rollconfirmation/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<RollConfirmationResponseDto>> GetRollConfirmation(int id)
		{
			try
			{
				var rollConfirmation = await _context.RollConfirmations.FindAsync(id);

				if (rollConfirmation == null)
				{
					return NotFound($"Roll confirmation with ID {id} not found.");
				}

				var responseDto = new RollConfirmationResponseDto
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
					CreatedDate = rollConfirmation.CreatedDate
				};

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching roll confirmation with ID {Id}", id);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/rollconfirmation/by-allot-id/{allotId}
		[HttpGet("by-allot-id/{allotId}")]
		public async Task<ActionResult<IEnumerable<RollConfirmationResponseDto>>> GetRollConfirmationsByAllotId(string allotId)
		{
			try
			{
				var rollConfirmations = await _context.RollConfirmations
					.Where(r => r.AllotId == allotId)
					.ToListAsync();

				if (rollConfirmations == null || rollConfirmations.Count == 0)
				{
					return NotFound($"No roll confirmations found for Allot ID {allotId}.");
				}


				var responseDtos = rollConfirmations.Select(roll => new RollConfirmationResponseDto
				{
					Id = roll.Id,
					AllotId = roll.AllotId,
					MachineName = roll.MachineName,
					RollPerKg = roll.RollPerKg,
					GreyGsm = roll.GreyGsm,
					GreyWidth = roll.GreyWidth,
					BlendPercent = roll.BlendPercent,
					Cotton = roll.Cotton,
					Polyester = roll.Polyester,
					Spandex = roll.Spandex,
					RollNo = roll.RollNo,
					GrossWeight = roll.GrossWeight,
					TareWeight = roll.TareWeight,
					NetWeight = roll.NetWeight,
					FgRollNo = roll.FgRollNo, // Include in response
					IsFGStickerGenerated = roll.IsFGStickerGenerated,
					CreatedDate = roll.CreatedDate
				}).ToList();

				return Ok(responseDtos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching roll confirmations for Allot ID {AllotId}", allotId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// GET api/rollconfirmation/weight-data - Get weight data from TCP client
		[HttpGet("weight-data")]
		public async Task<ActionResult<WeightDataResponseDto>> GetWeightData([FromQuery] string ipAddress, [FromQuery] int port = 23)
		 {
			try
			{
				using var client = new TcpClient();
				await client.ConnectAsync(ipAddress, port);
				var stream = client.GetStream();

				// Read the response
				byte[] buffer = new byte[256];
				int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

				if (bytesRead > 0)
				{
					string weightData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
					var parsedData = ParseWeightData(weightData);

					return Ok(parsedData);
				}
				else
				{
					var parsedData = ParseWeightData("0");
					return Ok(parsedData);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting weight data from TCP client {IpAddress}:{Port}", ipAddress, port);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		private WeightDataResponseDto ParseWeightData(string data)
		{
			var parts = data.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			string grossWeight = "0.00";
			string tareWeight = "0.00";
			string netWeight = "0.00";

			if (parts.Length >= 0)
			{
				grossWeight = parts[0];
			}
			else
			{
				grossWeight = 0f.ToString("F2");
				_logger.LogWarning("Failed to parse gross weight from data: {Data}", data);
			}


			return new WeightDataResponseDto
			{
				GrossWeight = grossWeight,
				TareWeight = tareWeight,
				NetWeight = netWeight
			};
		}

		// PUT api/rollconfirmation/{id} - Update roll confirmation with weight data
		[HttpPut("{id}")]
		public async Task<ActionResult<RollConfirmationResponseDto>> UpdateRollConfirmation(int id, [FromBody] RollConfirmationUpdateDto updateData)
		{
			try
			{
				var rollConfirmation = await _context.RollConfirmations.FindAsync(id);

				if (rollConfirmation == null)
				{
					return NotFound($"Roll confirmation with ID {id} not found.");
				}

				// Check if FG sticker has already been generated
				if (rollConfirmation.IsFGStickerGenerated && updateData.IsFGStickerGenerated.HasValue && updateData.IsFGStickerGenerated.Value)
				{
					return Conflict("FG Sticker has already been generated for this roll. Please scan next roll.");
				}

				// Update weight fields
				if (updateData.GrossWeight.HasValue)
					rollConfirmation.GrossWeight = updateData.GrossWeight.Value;

				if (updateData.TareWeight.HasValue)
					rollConfirmation.TareWeight = updateData.TareWeight.Value;

				if (updateData.NetWeight.HasValue)
					rollConfirmation.NetWeight = updateData.NetWeight.Value;

				// Update fabric specification fields
				if (updateData.GreyGsm.HasValue)
					rollConfirmation.GreyGsm = updateData.GreyGsm.Value;

				if (updateData.GreyWidth.HasValue)
					rollConfirmation.GreyWidth = updateData.GreyWidth.Value;

				if (updateData.BlendPercent.HasValue)
					rollConfirmation.BlendPercent = updateData.BlendPercent.Value;

				if (updateData.Cotton.HasValue)
					rollConfirmation.Cotton = updateData.Cotton.Value;

				if (updateData.Polyester.HasValue)
					rollConfirmation.Polyester = updateData.Polyester.Value;

				if (updateData.Spandex.HasValue)
					rollConfirmation.Spandex = updateData.Spandex.Value;

				// Update FG Sticker generated flag if provided
				if (updateData.IsFGStickerGenerated.HasValue)
				{
					rollConfirmation.IsFGStickerGenerated = updateData.IsFGStickerGenerated.Value;
				}

				// Automatically assign FG Roll Number if not provided
				int? fgRollNo = updateData.FgRollNo;
				if (!fgRollNo.HasValue)
				{
					// Get the maximum FG Roll Number for this AllotId and increment by 1
					var maxFgRollNo = await _context.RollConfirmations
						.Where(r => r.AllotId == rollConfirmation.AllotId)
						.MaxAsync(r => (int?)r.FgRollNo);

					fgRollNo = (maxFgRollNo ?? 0) + 1;
				}

				// Update FG Roll Number
				rollConfirmation.FgRollNo = fgRollNo;

				await _context.SaveChangesAsync();

				// Create response DTO
				var responseDto = new RollConfirmationResponseDto
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

				return Ok(responseDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating roll confirmation with ID {Id}", id);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
		
		// GET api/rollconfirmation - Get all roll confirmations
		[HttpGet]
		public async Task<ActionResult<IEnumerable<RollConfirmationResponseDto>>> GetAllRollConfirmations()
		{
			try
			{
				var rollConfirmations = await _context.RollConfirmations
					.ToListAsync();

				var responseDtos = rollConfirmations.Select(roll => new RollConfirmationResponseDto
				{
					Id = roll.Id,
					AllotId = roll.AllotId,
					MachineName = roll.MachineName,
					RollPerKg = roll.RollPerKg,
					GreyGsm = roll.GreyGsm,
					GreyWidth = roll.GreyWidth,
					BlendPercent = roll.BlendPercent,
					Cotton = roll.Cotton,
					Polyester = roll.Polyester,
					Spandex = roll.Spandex,
					RollNo = roll.RollNo,
					GrossWeight = roll.GrossWeight,
					TareWeight = roll.TareWeight,
					NetWeight = roll.NetWeight,
					FgRollNo = roll.FgRollNo,
					IsFGStickerGenerated = roll.IsFGStickerGenerated,
					CreatedDate = roll.CreatedDate
				}).ToList();

				return Ok(responseDtos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching all roll confirmations");
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}