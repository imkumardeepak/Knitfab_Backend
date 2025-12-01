using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.Report;
using AvyyanBackend.Models.ProAllot;
using AvyyanBackend.Models.ProductionConfirmation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportController> _logger;
        private readonly IMapper _mapper;

        public ReportController(ApplicationDbContext context, ILogger<ReportController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        // GET api/report/fabric-plan
        [HttpGet("fabric-plan")]
        public async Task<ActionResult<FabricPlanReportResponseDto>> GetFabricPlanReport([FromQuery] FabricPlanReportFilterDto filter)
        {
            try
            {
                var response = await GenerateFabricPlanReport(filter);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating fabric plan report");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/report/fabric-plan/filter-options
        [HttpGet("fabric-plan/filter-options")]
        public async Task<ActionResult<object>> GetFabricPlanFilterOptions()
        {
            try
            {
                var diaGgOptions = await _context.ProductionAllotments
                    .Select(pa => $"{pa.Diameter}/{pa.Gauge}")
                    .Distinct()
                    .OrderBy(dg => dg)
                    .ToListAsync();

                var customerOptions = await _context.ProductionAllotments
                    .Select(pa => pa.PartyName)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Distinct()
                    .OrderBy(name => name)
                    .ToListAsync();

                var yarnCountOptions = await _context.ProductionAllotments
                    .Select(pa => pa.YarnCount)
                    .Where(yarn => !string.IsNullOrEmpty(yarn))
                    .Distinct()
                    .OrderBy(yarn => yarn)
                    .ToListAsync();

                return Ok(new
                {
                    DiaGgOptions = diaGgOptions,
                    CustomerOptions = customerOptions,
                    YarnCountOptions = yarnCountOptions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching fabric plan filter options");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<FabricPlanReportResponseDto> GenerateFabricPlanReport(FabricPlanReportFilterDto filter)
        {
            // Get all production allotments with their machine allocations
            var query = _context.ProductionAllotments
                .Include(pa => pa.MachineAllocations)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.DiaGg))
            {
                var parts = filter.DiaGg.Split('/');
                if (parts.Length == 2 && int.TryParse(parts[0], out int dia) && int.TryParse(parts[1], out int gg))
                {
                    query = query.Where(pa => pa.Diameter == dia && pa.Gauge == gg);
                }
            }

            if (!string.IsNullOrEmpty(filter.CustomerName))
            {
                query = query.Where(pa => pa.PartyName == filter.CustomerName);
            }

            if (!string.IsNullOrEmpty(filter.YarnCount))
            {
                query = query.Where(pa => pa.YarnCount == filter.YarnCount);
            }

            // Apply date filters if provided
            if (filter.FromDate.HasValue)
            {
                query = query.Where(pa => pa.CreatedDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(pa => pa.CreatedDate <= filter.ToDate.Value.AddDays(1)); // Include the entire end date
            }

            var productionAllotments = await query.ToListAsync();

            // Get all roll confirmations
            var rollConfirmations = await _context.RollConfirmations.ToListAsync();

            var reports = new List<FabricPlanReportDto>();
            var summaries = new Dictionary<string, FabricPlanReportSummaryDto>();

            foreach (var allotment in productionAllotments)
            {
                var diaGg = $"{allotment.Diameter}/{allotment.Gauge}";
                
                // Calculate number of running machines
                var runningMachines = rollConfirmations
                    .Where(rc => rc.AllotId == allotment.AllotmentId)
                    .Select(rc => rc.MachineName)
                    .Distinct()
                    .Count();

                // Calculate sum of per day production
                var perDayProduction = CalculatePerDayProduction(allotment, rollConfirmations);

                // Calculate updated quantity (previous date created date except current date)
                var updatedQuantity = CalculateUpdatedQuantity(allotment, rollConfirmations);

                // Calculate balance quantity
                var balanceQuantity = allotment.ActualQuantity - updatedQuantity;

                // Estimate program completion date based on production time
                var programCompletionDate = allotment.CreatedDate.AddDays((double)allotment.MachineAllocations.Sum(ma => ma.EstimatedProductionTime));

                var report = new FabricPlanReportDto
                {
                    DiaGg = diaGg,
                    ProgramCompletionDate = programCompletionDate,
                    CustomerName = allotment.PartyName ?? "",
                    Count = !string.IsNullOrEmpty(allotment.YarnCount) ? ParseYarnCount(allotment.YarnCount) : 0,
                    FabricLotNo = allotment.AllotmentId,
                    NumberOfRunningMachines = runningMachines,
                    SumOfPerDayProduction = perDayProduction,
                    SumOfOrderQuantity = allotment.ActualQuantity,
                    SumOfUpdatedQuantity = updatedQuantity,
                    SumOfBalanceQuantity = balanceQuantity,
                    BalanceDays = balanceQuantity > 0 && perDayProduction > 0 ? (int)Math.Ceiling(balanceQuantity / perDayProduction) : 0
                };

                reports.Add(report);

                // Update summary
                if (!summaries.ContainsKey(diaGg))
                {
                    summaries[diaGg] = new FabricPlanReportSummaryDto
                    {
                        DiaGg = diaGg,
                        TotalPerDayProduction = 0,
                        TotalOrderQuantity = 0,
                        TotalUpdatedQuantity = 0,
                        TotalBalanceQuantity = 0,
                        TotalRunningMachines = 0
                    };
                }

                summaries[diaGg].TotalPerDayProduction += perDayProduction;
                summaries[diaGg].TotalOrderQuantity += allotment.ActualQuantity;
                summaries[diaGg].TotalUpdatedQuantity += updatedQuantity;
                summaries[diaGg].TotalBalanceQuantity += balanceQuantity;
                summaries[diaGg].TotalRunningMachines += runningMachines;
            }

            // Sort reports by Dia/GG
            reports = reports.OrderBy(r => r.DiaGg).ToList();

            // Calculate grand total
            var grandTotal = new FabricPlanReportSummaryDto
            {
                DiaGg = "Grand Total",
                TotalPerDayProduction = summaries.Values.Sum(s => s.TotalPerDayProduction),
                TotalOrderQuantity = summaries.Values.Sum(s => s.TotalOrderQuantity),
                TotalUpdatedQuantity = summaries.Values.Sum(s => s.TotalUpdatedQuantity),
                TotalBalanceQuantity = summaries.Values.Sum(s => s.TotalBalanceQuantity),
                TotalRunningMachines = summaries.Values.Sum(s => s.TotalRunningMachines)
            };

            return new FabricPlanReportResponseDto
            {
                Reports = reports,
                Summaries = summaries.Values.OrderBy(s => s.DiaGg).ToList(),
                GrandTotal = grandTotal
            };
        }

        private decimal CalculatePerDayProduction(ProductionAllotment allotment, List<RollConfirmation> rollConfirmations)
        {
            // Check roll confirmation table for any roll with net weight for the lot
            var rollsForLot = rollConfirmations.Where(rc => rc.AllotId == allotment.AllotmentId).ToList();

            if (!rollsForLot.Any())
            {
                return 0; // No rolls exist
            }

            // Check if any roll has net weight
            var rollsWithNetWeight = rollsForLot.Where(rc => rc.NetWeight.HasValue && rc.NetWeight.Value > 0).ToList();

            if (rollsWithNetWeight.Any())
            {
                // Sum of net weights
                return rollsWithNetWeight.Sum(rc => rc.NetWeight.Value);
            }

            // If rolls exist but no net weight, use default wtper roll from production allotment table
            var machineAllocation = allotment.MachineAllocations.FirstOrDefault();
            if (machineAllocation != null)
            {
                // Calculate total production based on roll breakdown
                return machineAllocation.TotalRolls * machineAllocation.RollPerKg;
            }

            return 0;
        }

        private decimal CalculateUpdatedQuantity(ProductionAllotment allotment, List<RollConfirmation> rollConfirmations)
        {
            // Sum of updated quantity: previous date created date except current date
            var currentDate = DateTime.UtcNow.Date;
            return rollConfirmations
                .Where(rc => rc.AllotId == allotment.AllotmentId && 
                             rc.CreatedDate.Date < currentDate)
                .Sum(rc => rc.NetWeight ?? 0);
        }

        private int ParseYarnCount(string yarnCount)
        {
            // Parse yarn count like "24/1 CCH" to get the numeric part
            if (string.IsNullOrEmpty(yarnCount))
                return 0;

            var parts = yarnCount.Split(' ');
            if (parts.Length > 0)
            {
                var numberParts = parts[0].Split('/');
                if (numberParts.Length > 0 && int.TryParse(numberParts[0], out int count))
                {
                    return count;
                }
            }

            return 0;
        }
    }
}