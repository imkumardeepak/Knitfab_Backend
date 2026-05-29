using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.ProductionConfirmation;
using AvyyanBackend.Models.ProductionConfirmation;
using Microsoft.AspNetCore.Mvc;
using AvyyanBackend.Filters;
using AvyyanBackend.Interfaces;
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
        private readonly IAuditLogService _auditLogService;

        public InspectionController(ApplicationDbContext context, ILogger<InspectionController> logger, IMapper mapper, IAuditLogService auditLogService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        // POST api/inspection
        [HttpPost]
        public async Task<ActionResult<InspectionResponseDto>> CreateInspection([FromBody] InspectionRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (request.Status == InspectionStatus.Accepted && string.IsNullOrWhiteSpace(request.Grade))
                    return BadRequest("Grade is required when accepting a roll.");

                if (request.Status == InspectionStatus.Rejected && string.IsNullOrWhiteSpace(request.Remarks))
                    return BadRequest("Remarks are required when rejecting a roll.");

                var existingInspection = await _context.Inspections
                    .FirstOrDefaultAsync(i => i.AllotId == request.AllotId &&
                                         i.MachineName == request.MachineName &&
                                         i.RollNo == request.RollNo);

                if (existingInspection != null)
                    return Conflict($"Inspection for Allot ID {request.AllotId}, Machine {request.MachineName}, Roll No {request.RollNo} already exists.");

                var rollConfirmation = await _context.RollConfirmations
                    .FirstOrDefaultAsync(r => r.AllotId == request.AllotId &&
                                         r.MachineName == request.MachineName &&
                                         r.RollNo == request.RollNo);

                if (rollConfirmation == null)
                    return BadRequest("Roll must be confirmed before inspection. Please confirm the roll first, then inspect it.");

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
                    Status = request.Status
                };

                _context.Inspections.Add(inspection);
                await _context.SaveChangesAsync();

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
                    Status = inspection.Status
                };

                await _auditLogService.LogAsync(
                    action: $"INSPECTION_{inspection.Status.ToString().ToUpperInvariant()}",
                    module: "Inspection",
                    entityId: inspection.Id,
                    entityName: inspection.AllotId,
                    changeSummary: inspection.Status switch
                    {
                        InspectionStatus.Accepted => $"Roll #{inspection.RollNo} ACCEPTED in Allot {inspection.AllotId} - Grade: {inspection.Grade}, Faults: {inspection.TotalFaults}",
                        InspectionStatus.Rejected => $"Roll #{inspection.RollNo} REJECTED in Allot {inspection.AllotId} - Reason: {inspection.Remarks}",
                        InspectionStatus.Hold => $"Roll #{inspection.RollNo} HELD in Allot {inspection.AllotId} - Remarks: {inspection.Remarks}",
                        _ => $"Roll #{inspection.RollNo} inspection updated in Allot {inspection.AllotId}"
                    },
                    newValues: new { inspection.AllotId, inspection.MachineName, inspection.RollNo, inspection.Grade, inspection.TotalFaults, inspection.Status, inspection.Remarks }
                );

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
                    return NotFound($"Inspection with ID {id} not found.");

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
                    Status = inspection.Status
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
                    Status = inspection.Status
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
