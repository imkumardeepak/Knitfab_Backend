using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.ProAllotDto;
using AvyyanBackend.Models.ProAllot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RollAssignmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RollAssignmentController> _logger;
        private readonly IMapper _mapper;

        public RollAssignmentController(ApplicationDbContext context, ILogger<RollAssignmentController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        // POST api/rollassignment
        [HttpPost]
        public async Task<ActionResult<RollAssignmentResponseDto>> CreateRollAssignment([FromBody] CreateRollAssignmentRequest request)
        {
            try
            {
                // Validate request
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });
                    return BadRequest(new { Message = "Validation failed", Errors = errors });
                }

                // Additional validation
                if (request.MachineAllocationId <= 0)
                    return BadRequest("Machine allocation ID must be greater than 0");
                
                if (request.ShiftId <= 0)
                    return BadRequest("Shift ID must be greater than 0");
                    
                if (request.AssignedRolls < 0)
                    return BadRequest("Assigned rolls must be greater than or equal to 0");
                    
                if (string.IsNullOrWhiteSpace(request.OperatorName))
                    return BadRequest("Operator name is required");

                // Check if machine allocation exists
                var machineAllocation = await _context.MachineAllocations
                    .FirstOrDefaultAsync(m => m.Id == request.MachineAllocationId);
                
                if (machineAllocation == null)
                {
                    return NotFound($"Machine allocation with ID {request.MachineAllocationId} not found.");
                }

                // Calculate total assigned rolls for this machine allocation
                var totalAssigned = await _context.RollAssignments
                    .Where(ra => ra.MachineAllocationId == request.MachineAllocationId)
                    .SumAsync(ra => ra.AssignedRolls);

                // Check if assigned rolls exceed remaining rolls
                var remainingRolls = machineAllocation.TotalRolls - totalAssigned;
                if (request.AssignedRolls > remainingRolls)
                {
                    return BadRequest($"Cannot assign {request.AssignedRolls} rolls. Only {remainingRolls} rolls remaining.");
                }
               

                // Create roll assignment entity
                var rollAssignment = new RollAssignment
                {
                    MachineAllocationId = request.MachineAllocationId,
                    ShiftId = request.ShiftId,
                    AssignedRolls = request.AssignedRolls,
                    GeneratedStickers = 0,
                    RemainingRolls = request.AssignedRolls,
                    OperatorName = request.OperatorName,
                    Timestamp = request.Timestamp
                };

                // Add to database
                _context.RollAssignments.Add(rollAssignment);
                await _context.SaveChangesAsync();

                // Create response DTO
                var responseDto = new RollAssignmentResponseDto
                {
                    Id = rollAssignment.Id,
                    MachineAllocationId = rollAssignment.MachineAllocationId,
                    ShiftId = rollAssignment.ShiftId,
                    AssignedRolls = rollAssignment.AssignedRolls,
                    GeneratedStickers = rollAssignment.GeneratedStickers,
                    RemainingRolls = rollAssignment.RemainingRolls,
                    OperatorName = rollAssignment.OperatorName,
                    Timestamp = rollAssignment.Timestamp
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating roll assignment");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        // POST api/rollassignment/generate-stickers
        [HttpPost("generate-stickers")]
        public async Task<ActionResult<RollAssignmentResponseDto>> GenerateStickers([FromBody] GenerateStickersRequest request)
        {
            try
            {
                // Validate request
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });
                    return BadRequest(new { Message = "Validation failed", Errors = errors });
                }
                
                // Additional validation
                if (request.RollAssignmentId <= 0)
                    return BadRequest("Roll assignment ID must be greater than 0");
                    
                if (request.StickerCount < 0)
                    return BadRequest("Sticker count must be greater than or equal to 0");

                // Find the roll assignment
                var rollAssignment = await _context.RollAssignments
                    .FirstOrDefaultAsync(ra => ra.Id == request.RollAssignmentId);
                
                if (rollAssignment == null)
                {
                    return NotFound($"Roll assignment with ID {request.RollAssignmentId} not found.");
                }

                // Check if requested sticker count exceeds remaining rolls
                if (request.StickerCount > rollAssignment.RemainingRolls)
                {
                    return BadRequest($"Cannot generate {request.StickerCount} stickers. Only {rollAssignment.RemainingRolls} rolls remaining.");
                }

                // Update roll assignment
                rollAssignment.GeneratedStickers += request.StickerCount;
                rollAssignment.RemainingRolls -= request.StickerCount;
                
                _context.RollAssignments.Update(rollAssignment);
                await _context.SaveChangesAsync();

                // Create response DTO
                var responseDto = new RollAssignmentResponseDto
                {
                    Id = rollAssignment.Id,
                    MachineAllocationId = rollAssignment.MachineAllocationId,
                    ShiftId = rollAssignment.ShiftId,
                    AssignedRolls = rollAssignment.AssignedRolls,
                    GeneratedStickers = rollAssignment.GeneratedStickers,
                    RemainingRolls = rollAssignment.RemainingRolls,
                    OperatorName = rollAssignment.OperatorName,
                    Timestamp = rollAssignment.Timestamp
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating stickers");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        // GET api/rollassignment/by-machine-allocation/{machineAllocationId}
        [HttpGet("by-machine-allocation/{machineAllocationId}")]
        public async Task<ActionResult<IEnumerable<RollAssignmentResponseDto>>> GetRollAssignmentsByMachineAllocationId(int machineAllocationId)
        {
            try
            {
                // Validate parameter
                if (machineAllocationId <= 0)
                    return BadRequest("Machine allocation ID must be greater than 0");

                var rollAssignments = await _context.RollAssignments
                    .Include(ra => ra.GeneratedBarcodes)
                    .Where(ra => ra.MachineAllocationId == machineAllocationId)
                    .ToListAsync();

                var responseDtos = rollAssignments.Select(roll => new RollAssignmentResponseDto
                {
                    Id = roll.Id,
                    MachineAllocationId = roll.MachineAllocationId,
                    ShiftId = roll.ShiftId,
                    AssignedRolls = roll.AssignedRolls,
                    GeneratedStickers = roll.GeneratedStickers,
                    RemainingRolls = roll.RemainingRolls,
                    OperatorName = roll.OperatorName,
                    Timestamp = roll.Timestamp,
                    GeneratedBarcodes = roll.GeneratedBarcodes.Select(gb => new GeneratedBarcodeDto
                    {
                        Id = gb.Id,
                        RollAssignmentId = (int)gb.RollAssignmentId,
                        Barcode = gb.Barcode,
                        RollNumber = gb.RollNumber,
                        GeneratedAt = gb.GeneratedAt
                    }).ToList()
                }).ToList();

                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching roll assignments for Machine Allocation ID {MachineAllocationId}", machineAllocationId);
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        // POST api/rollassignment/generate-barcodes
        [HttpPost("generate-barcodes")]
        public async Task<ActionResult<RollAssignmentResponseDto>> GenerateBarcodes([FromBody] GenerateBarcodesRequest request)
        {
            try
            {
                // Validate request
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });
                    return BadRequest(new { Message = "Validation failed", Errors = errors });
                }

                // Additional validation
                if (request.RollAssignmentId <= 0)
                    return BadRequest("Roll assignment ID must be greater than 0");

                if (request.BarcodeCount <= 0)
                    return BadRequest("Barcode count must be greater than 0");

                // Find the roll assignment with generated barcodes
                var rollAssignment = await _context.RollAssignments
                    .Include(ra => ra.GeneratedBarcodes)
                    .FirstOrDefaultAsync(ra => ra.Id == request.RollAssignmentId);

                if (rollAssignment == null)
                {
                    return NotFound($"Roll assignment with ID {request.RollAssignmentId} not found.");
                }

                // Check if requested barcode count exceeds remaining rolls
                var remainingRolls = rollAssignment.RemainingRolls;
                if (request.BarcodeCount > remainingRolls)
                {
                    return BadRequest($"Cannot generate {request.BarcodeCount} barcodes. Only {remainingRolls} rolls remaining.");
                }

                // Get the next roll number for this machine allocation
                var nextRollNumber = 1;
                if (rollAssignment.GeneratedBarcodes.Any())
                {
                    nextRollNumber = rollAssignment.GeneratedBarcodes.Max(gb => gb.RollNumber) + 1;
                }
                else
                {
                    //  Check if there are any existing barcodes for this machine allocation to maintain continuity

                    var maxRollNumber = await _context.GeneratedBarcodes
                        .Where(gb => gb.RollAssignment.MachineAllocationId == rollAssignment.MachineAllocationId)
                        .MaxAsync(gb => (int?)gb.RollNumber) ?? 0;

                    nextRollNumber = maxRollNumber + 1;
                }

                // Generate barcodes
                var generatedBarcodes = new List<GeneratedBarcode>();
                for (int i = 0; i < request.BarcodeCount; i++)
                {
                    var barcode = new GeneratedBarcode
                    {
                        RollAssignmentId = rollAssignment.Id,
                        Barcode = $"{rollAssignment.MachineAllocationId}-{nextRollNumber + i}",
                        RollNumber = nextRollNumber + i,
                        GeneratedAt = DateTime.UtcNow
                    };

                    generatedBarcodes.Add(barcode);
                    _context.GeneratedBarcodes.Add(barcode);
                }

                // Update roll assignment
                rollAssignment.GeneratedStickers += request.BarcodeCount;
                rollAssignment.RemainingRolls -= request.BarcodeCount;

                _context.RollAssignments.Update(rollAssignment);
                await _context.SaveChangesAsync();

                // Create response DTO
                var responseDto = new RollAssignmentResponseDto
                {
                    Id = rollAssignment.Id,
                    MachineAllocationId = rollAssignment.MachineAllocationId,
                    ShiftId = rollAssignment.ShiftId,
                    AssignedRolls = rollAssignment.AssignedRolls,
                    GeneratedStickers = rollAssignment.GeneratedStickers,
                    RemainingRolls = rollAssignment.RemainingRolls,
                    OperatorName = rollAssignment.OperatorName,
                    Timestamp = rollAssignment.Timestamp,
                    GeneratedBarcodes = generatedBarcodes.Select(gb => new GeneratedBarcodeDto
                    {
                        Id = gb.Id,
                        RollAssignmentId = (int)gb.RollAssignmentId,
                        Barcode = gb.Barcode,
                        RollNumber = gb.RollNumber,
                        GeneratedAt = gb.GeneratedAt
                    }).ToList()
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating barcodes");
                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message, StackTrace = ex.StackTrace });
            }
        }
    }
}