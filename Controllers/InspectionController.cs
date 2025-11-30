using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.ProductionConfirmation;
using AvyyanBackend.Models.ProductionConfirmation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InspectionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InspectionController> _logger;
        private readonly IMapper _mapper;

        public InspectionController(ApplicationDbContext context, ILogger<InspectionController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        // POST api/inspection
        [HttpPost]
        public async Task<ActionResult<InspectionResponseDto>> CreateInspection([FromBody] InspectionRequestDto request)
        {
            try
            {
                // Validate request
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Require remarks when rejecting a roll (flag is false)
                if (!request.Flag && string.IsNullOrWhiteSpace(request.Remarks))
                {
                    return BadRequest("Remarks are required when rejecting a roll.");
                }

                // Check if this inspection already exists
                var existingInspection = await _context.Inspections
                    .FirstOrDefaultAsync(i => i.AllotId == request.AllotId && 
                                         i.MachineName == request.MachineName && 
                                         i.RollNo == request.RollNo);
                
                if (existingInspection != null)
                {
                    return Conflict($"Inspection for Allot ID {request.AllotId}, Machine {request.MachineName}, Roll No {request.RollNo} already exists.");
                }

                // Check if the roll has been confirmed before allowing inspection
                var rollConfirmation = await _context.RollConfirmations
                    .FirstOrDefaultAsync(r => r.AllotId == request.AllotId && 
                                         r.MachineName == request.MachineName && 
                                         r.RollNo == request.RollNo);
                
                if (rollConfirmation == null)
                {
                    return BadRequest("Roll must be confirmed before inspection. Please confirm the roll first, then inspect it.");
                }

                // Create inspection entity
                var inspection = new Inspection
                {
                    AllotId = request.AllotId,
                    MachineName = request.MachineName,
                    RollNo = request.RollNo,
                    ThinPlaces = request.ThinPlaces,
                    ThickPlaces = request.ThickPlaces,
                    ThinLines = request.ThinLines,
                    ThickLines = request.ThickLines,
                    DoubleParallelYarn = request.DoubleParallelYarn,
                    HaidJute = request.HaidJute,
                    ColourFabric = request.ColourFabric,
                    Holes = request.Holes,
                    DropStitch = request.DropStitch,
                    LycraStitch = request.LycraStitch,
                    LycraBreak = request.LycraBreak,
                    FFD = request.FFD,
                    NeedleBroken = request.NeedleBroken,
                    KnitFly = request.KnitFly,
                    OilSpots = request.OilSpots,
                    OilLines = request.OilLines,
                    VerticalLines = request.VerticalLines,
                    Grade = request.Grade,
                    TotalFaults = request.TotalFaults,
                    Remarks = request.Remarks,
                    CreatedDate = request.CreatedDate,
                    Flag = request.Flag // Add the flag field
                };

                // Add to database
                _context.Inspections.Add(inspection);
                await _context.SaveChangesAsync();

                // Create response DTO
                var responseDto = new InspectionResponseDto
                {
                    Id = inspection.Id,
                    AllotId = inspection.AllotId,
                    MachineName = inspection.MachineName,
                    RollNo = inspection.RollNo,
                    ThinPlaces = inspection.ThinPlaces,
                    ThickPlaces = inspection.ThickPlaces,
                    ThinLines = inspection.ThinLines,
                    ThickLines = inspection.ThickLines,
                    DoubleParallelYarn = inspection.DoubleParallelYarn,
                    HaidJute = inspection.HaidJute,
                    ColourFabric = inspection.ColourFabric,
                    Holes = inspection.Holes,
                    DropStitch = inspection.DropStitch,
                    LycraStitch = inspection.LycraStitch,
                    LycraBreak = inspection.LycraBreak,
                    FFD = inspection.FFD,
                    NeedleBroken = inspection.NeedleBroken,
                    KnitFly = inspection.KnitFly,
                    OilSpots = inspection.OilSpots,
                    OilLines = inspection.OilLines,
                    VerticalLines = inspection.VerticalLines,
                    Grade = inspection.Grade,
                    TotalFaults = inspection.TotalFaults,
                    Remarks = inspection.Remarks,
                    CreatedDate = inspection.CreatedDate,
                    Flag = inspection.Flag // Add the flag field
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inspection");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/inspection/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<InspectionResponseDto>> GetInspection(int id)
        {
            try
            {
                var inspection = await _context.Inspections.FindAsync(id);
                
                if (inspection == null)
                {
                    return NotFound($"Inspection with ID {id} not found.");
                }
                var responseDto = new InspectionResponseDto
                {
                    Id = inspection.Id,
                    AllotId = inspection.AllotId,
                    MachineName = inspection.MachineName,
                    RollNo = inspection.RollNo,
                    ThinPlaces = inspection.ThinPlaces,
                    ThickPlaces = inspection.ThickPlaces,
                    ThinLines = inspection.ThinLines,
                    ThickLines = inspection.ThickLines,
                    DoubleParallelYarn = inspection.DoubleParallelYarn,
                    HaidJute = inspection.HaidJute,
                    ColourFabric = inspection.ColourFabric,
                    Holes = inspection.Holes,
                    DropStitch = inspection.DropStitch,
                    LycraStitch = inspection.LycraStitch,
                    LycraBreak = inspection.LycraBreak,
                    FFD = inspection.FFD,
                    NeedleBroken = inspection.NeedleBroken,
                    KnitFly = inspection.KnitFly,
                    OilSpots = inspection.OilSpots,
                    OilLines = inspection.OilLines,
                    VerticalLines = inspection.VerticalLines,
                    Grade = inspection.Grade,
                    TotalFaults = inspection.TotalFaults,
                    Remarks = inspection.Remarks,
                    CreatedDate = inspection.CreatedDate,
                    Flag = inspection.Flag // Add the flag field
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inspection with ID {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/inspection/by-allot-id/{allotId}
        [HttpGet("by-allot-id/{allotId}")]
        public async Task<ActionResult<IEnumerable<InspectionResponseDto>>> GetInspectionsByAllotId(string allotId)
        {
            try
            {
                var inspections = await _context.Inspections
                    .Where(i => i.AllotId == allotId)
                    .ToListAsync();

                var responseDtos = inspections.Select(inspection => new InspectionResponseDto
                {
                    Id = inspection.Id,
                    AllotId = inspection.AllotId,
                    MachineName = inspection.MachineName,
                    RollNo = inspection.RollNo,
                    ThinPlaces = inspection.ThinPlaces,
                    ThickPlaces = inspection.ThickPlaces,
                    ThinLines = inspection.ThinLines,
                    ThickLines = inspection.ThickLines,
                    DoubleParallelYarn = inspection.DoubleParallelYarn,
                    HaidJute = inspection.HaidJute,
                    ColourFabric = inspection.ColourFabric,
                    Holes = inspection.Holes,
                    DropStitch = inspection.DropStitch,
                    LycraStitch = inspection.LycraStitch,
                    LycraBreak = inspection.LycraBreak,
                    FFD = inspection.FFD,
                    NeedleBroken = inspection.NeedleBroken,
                    KnitFly = inspection.KnitFly,
                    OilSpots = inspection.OilSpots,
                    OilLines = inspection.OilLines,
                    VerticalLines = inspection.VerticalLines,
                    Grade = inspection.Grade,
                    TotalFaults = inspection.TotalFaults,
                    Remarks = inspection.Remarks,
                    CreatedDate = inspection.CreatedDate,
                    Flag = inspection.Flag // Add the flag field
                }).ToList();

                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inspections for Allot ID {AllotId}", allotId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}