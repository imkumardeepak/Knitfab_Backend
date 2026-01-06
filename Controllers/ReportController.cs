using System.Collections.Generic;
using System.Threading.Tasks;
using AvyyanBackend.DTOs.Report;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("final-fabric-report")]
        public async Task<ActionResult<List<FinalFabricReportDto>>> GetFinalFabricReport()
        {
            var report = await _reportService.GetFinalFabricReportAsync();
            return Ok(report);
        }

        [HttpGet("final-fabric-report/{salesOrderId}")]
        public async Task<ActionResult<FinalFabricReportDto>> GetFinalFabricReportBySalesOrder(int salesOrderId)
        {
            var report = await _reportService.GetFinalFabricReportBySalesOrderAsync(salesOrderId);
            if (report == null)
            {
                return NotFound($"Sales order with ID {salesOrderId} not found.");
            }
            return Ok(report);
        }
    }
}