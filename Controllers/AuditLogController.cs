using AvyyanBackend.DTOs.AuditLog;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(IAuditLogService auditLogService, ILogger<AuditLogController> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated audit logs with optional filters.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<AuditLogPagedResultDto>> GetLogs([FromQuery] AuditLogQueryDto query)
        {
            try
            {
                if (query.PageSize > 200) query.PageSize = 200; // hard cap
                var result = await _auditLogService.GetLogsAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching audit logs");
                return StatusCode(500, "An error occurred while fetching audit logs");
            }
        }

        /// <summary>
        /// Get list of distinct modules (for filter dropdown).
        /// </summary>
        [HttpGet("modules")]
        public async Task<ActionResult<List<string>>> GetModules()
        {
            try
            {
                var modules = await _auditLogService.GetModulesAsync();
                return Ok(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching audit log modules");
                return StatusCode(500, "An error occurred while fetching modules");
            }
        }

        /// <summary>
        /// Get list of distinct action types (for filter dropdown).
        /// </summary>
        [HttpGet("actions")]
        public async Task<ActionResult<List<string>>> GetActions()
        {
            try
            {
                var actions = await _auditLogService.GetActionsAsync();
                return Ok(actions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching audit log actions");
                return StatusCode(500, "An error occurred while fetching actions");
            }
        }
    }
}
