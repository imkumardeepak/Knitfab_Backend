using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.Report;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using AvyyanBackend.Models.ProAllot;
using AvyyanBackend.Models.ProductionConfirmation;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FinalFabricReportDto>> GetFinalFabricReportAsync()
        {
            // 1. Fetch all root data
            var salesOrders = await _context.SalesOrdersWeb
                .Include(so => so.Items)
                .AsNoTracking()
                .ToListAsync();

            var salesOrderIds = salesOrders.Select(so => so.Id).ToList();
            var voucherNumbers = salesOrders.Select(so => so.VoucherNumber).Where(v => !string.IsNullOrEmpty(v)).ToList();

            // 2. Fetch all Production Allotments matching either ID OR VoucherNumber for robust coverage
            var productionAllotments = await _context.ProductionAllotments
                .Include(pa => pa.MachineAllocations)
                .Where(pa => salesOrderIds.Contains(pa.SalesOrderId) || voucherNumbers.Contains(pa.VoucherNumber))
                .AsNoTracking()
                .ToListAsync();

            var allotmentIds = productionAllotments.Select(pa => pa.AllotmentId).ToList();
            var yarnLotNos = productionAllotments.Select(pa => pa.YarnLotNo).Where(l => !string.IsNullOrEmpty(l)).Distinct().ToList();
            // var yarnLotNos = productionAllotments.Select(pa => pa.YarnLotNo).Where(l => !string.IsNullOrEmpty(l)).Distinct().ToList(); // Removed as per instruction

            // 3. Fetch confirmations and dispatch details for the matched lots
            var rollConfirmations = await _context.RollConfirmations
                .Where(rc => allotmentIds.Contains(rc.AllotId))
                .AsNoTracking()
                .ToListAsync();

            var dispatchPlannings = await _context.DispatchPlannings
                .Where(dp => allotmentIds.Contains(dp.LotNo)) // Changed to use allotmentIds
                .AsNoTracking()
                .ToListAsync();

            // 4. Create lookups for fast in-memory joining
            var rcLookup = rollConfirmations.GroupBy(rc => rc.AllotId).ToDictionary(g => g.Key, g => g.ToList());
            var dpLookup = dispatchPlannings.GroupBy(dp => dp.LotNo).ToDictionary(g => g.Key, g => g.ToList());

            var reports = new List<FinalFabricReportDto>();

            foreach (var so in salesOrders)
            {
                var report = new FinalFabricReportDto
                {
                    SalesOrderId = so.Id,
                    VoucherNumber = so.VoucherNumber,
                    BuyerName = so.BuyerName,
                    OrderDate = so.OrderDate,
                    SalesOrderItems = new List<SalesOrderItemReportDto>()
                };

                // Get all allotmetns associated with THIS sales order (by ID or Voucher)
                var soAllotments = productionAllotments
                    .Where(pa => pa.SalesOrderId == so.Id || (pa.VoucherNumber == so.VoucherNumber && !string.IsNullOrEmpty(so.VoucherNumber)))
                    .ToList();

                var processedAllotmentIds = new HashSet<int>();

                // Map allotmetns to existing items
                foreach (var item in so.Items)
                {
                    var itemReport = new SalesOrderItemReportDto
                    {
                        SalesOrderItemId = item.Id,
                        ItemName = item.ItemName,
                        YarnCount = item.YarnCount,
                        Dia = item.Dia,
                        GG = item.GG,
                        FabricType = item.FabricType,
                        Qty = item.Qty,
                        ProductionAllotments = new List<ProductionAllotmentReportDto>()
                    };

                    // First try matching by Item ID
                    var itemAllotments = soAllotments.Where(pa => pa.SalesOrderItemId == item.Id).ToList();
                    
                    // If no match by ID, try matching by Item Name if the Voucher matches
                    if (!itemAllotments.Any())
                    {
                        itemAllotments = soAllotments.Where(pa => pa.ItemName == item.ItemName && !processedAllotmentIds.Contains(pa.Id)).ToList();
                    }

                    foreach (var pa in itemAllotments)
                    {
                        var paReport = MapAllotmentToReport(pa, rcLookup, dpLookup);
                        itemReport.ProductionAllotments.Add(paReport);
                        processedAllotmentIds.Add(pa.Id);
                    }

                    report.SalesOrderItems.Add(itemReport);
                }

                // IMPORTANT: Identify "Orphan" allotments linked to the sales order but not to any specific item
                var orphanAllotments = soAllotments.Where(pa => !processedAllotmentIds.Contains(pa.Id)).ToList();
                if (orphanAllotments.Any())
                {
                    foreach (var pa in orphanAllotments)
                    {
                        // Create a "virtual" item entry for these lots
                        var orphanItem = new SalesOrderItemReportDto
                        {
                            SalesOrderItemId = 0, // Mark as unmatched
                            ItemName = pa.ItemName + " (Unmatched Lot)",
                            YarnCount = pa.YarnCount,
                            Dia = pa.Diameter,
                            GG = pa.Gauge,
                            FabricType = pa.FabricType,
                            Qty = pa.ActualQuantity,
                            ProductionAllotments = new List<ProductionAllotmentReportDto> { MapAllotmentToReport(pa, rcLookup, dpLookup) }
                        };
                        report.SalesOrderItems.Add(orphanItem);
                    }
                }

                reports.Add(report);
            }

            return reports;
        }

        private ProductionAllotmentReportDto MapAllotmentToReport(ProductionAllotment pa, Dictionary<string, List<RollConfirmation>> rcLookup, Dictionary<string, List<DispatchPlanning>> dpLookup)
        {
            var paReport = new ProductionAllotmentReportDto
            {
                ProductionAllotmentId = pa.Id,
                AllotmentId = pa.AllotmentId,
                YarnCount = pa.YarnCount,
                Diameter = pa.Diameter,
                Gauge = pa.Gauge,
                FabricType = pa.FabricType,
                PartyName = pa.PartyName,
                YarnPartyName = pa.YarnPartyName,
                YarnLotNo = pa.YarnLotNo,
                ActualQuantity = pa.ActualQuantity,
                TotalRunningMachines = pa.MachineAllocations?.Count ?? 0,
                MachineAllocations = pa.MachineAllocations?.Select(ma => new MachineAllocationReportDto
                {
                    MachineAllocationId = ma.Id,
                    MachineName = ma.MachineName,
                    NumberOfNeedles = ma.NumberOfNeedles,
                    Feeders = ma.Feeders,
                    RPM = ma.RPM,
                    TotalRolls = (int)ma.TotalRolls
                }).ToList() ?? new List<MachineAllocationReportDto>()
            };

            if (rcLookup.TryGetValue(pa.AllotmentId, out var rcs))
            {
                paReport.TotalConfirmedRolls = rcs.Count;
                paReport.TotalConfirmedNetWeight = rcs.Sum(rc => rc.NetWeight ?? 0);
                paReport.RollConfirmations = rcs.Select(rc => new RollConfirmationReportDto
                {
                    RollConfirmationId = rc.Id,
                    RollNo = rc.RollNo,
                    MachineName = rc.MachineName,
                    NetWeight = rc.NetWeight ?? 0,
                    GreyGsm = rc.GreyGsm,
                    GreyWidth = rc.GreyWidth,
                    FgRollNo = rc.FgRollNo,
                    CreatedDate = rc.CreatedDate
                }).ToList();
            }

            if (dpLookup.TryGetValue(pa.AllotmentId, out var dps))
            {
                paReport.TotalDispatchedRolls = (int)dps.Sum(dp => dp.TotalDispatchedRolls);
                paReport.TotalDispatchedNetWeight = dps.Sum(dp => dp.TotalNetWeight ?? 0);
                paReport.DispatchPlannings = dps.Select(dp => new DispatchPlanningReportDto
                {
                    DispatchPlanningId = dp.Id,
                    LotNo = dp.LotNo,
                    CustomerName = dp.CustomerName,
                    TotalRequiredRolls = dp.TotalRequiredRolls,
                    TotalReadyRolls = dp.TotalReadyRolls,
                    TotalDispatchedRolls = dp.TotalDispatchedRolls,
                    TotalNetWeight = dp.TotalNetWeight,
                    VehicleNo = dp.VehicleNo,
                    DispatchStartDate = dp.DispatchStartDate,
                    DispatchEndDate = dp.DispatchEndDate
                }).ToList();
            }

            return paReport;
        }

        public async Task<List<FabricStockReportDto>> GetFabricStockReportAsync()
        {
            // 1. Fetch all root data in batch
            var allotments = await _context.ProductionAllotments
                .Include(pa => pa.MachineAllocations)
                .AsNoTracking()
                .ToListAsync();

            var allotmentIds = allotments.Select(pa => pa.AllotmentId).ToList();

            // 2. Fetch related data in bulk
            var rollConfirmations = await _context.RollConfirmations
                .Where(rc => allotmentIds.Contains(rc.AllotId))
                .AsNoTracking()
                .ToListAsync();

            var storageCaptures = await _context.StorageCaptures
                .Where(sc => allotmentIds.Contains(sc.LotNo))
                .AsNoTracking()
                .ToListAsync();

            // 3. Create lookups for fast in-memory joining
            var rcLookup = rollConfirmations
                .GroupBy(rc => rc.AllotId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var scLookup = storageCaptures
                .GroupBy(sc => sc.LotNo)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 4. Construct DTOs in memory
            var reportData = new List<FabricStockReportDto>();

            foreach (var pa in allotments)
            {
                rcLookup.TryGetValue(pa.AllotmentId, out var lotRcs);
                scLookup.TryGetValue(pa.AllotmentId, out var lotScs);

                var updateQuantity = lotRcs?.Sum(rc => rc.NetWeight ?? 0) ?? 0;
                var updatedNoOfRolls = lotScs?.Count ?? 0;
                var dispatchedRolls = lotScs?.Count(sc => sc.IsDispatched == true) ?? 0;
                var stockRolls = lotScs?.Count(sc => sc.IsDispatched == false) ?? 0;
                
                var inStockRolls = lotRcs?.Where(rc => rc.FgRollNo != null && 
                    !(lotScs?.FirstOrDefault(sc => sc.FGRollNo == rc.FgRollNo.ToString())?.IsDispatched == true))
                    .ToList() ?? new List<RollConfirmation>();

                var availableQuantity = inStockRolls.Count;
                var availableWeight = inStockRolls.Sum(rc => rc.NetWeight ?? 0m);

                var allocatedRolls = pa.MachineAllocations?.Sum(ma => ma.TotalRolls) ?? 0;
                var requiredRolls = (decimal)allocatedRolls;

                var dto = new FabricStockReportDto
                {
                    LotNo = pa.AllotmentId,
                    VoucherNumber = pa.VoucherNumber,
                    ItemName = pa.ItemName,
                    CustomerName = pa.PartyName ?? "Unknown",
                    OrderQuantity = pa.ActualQuantity,
                    RequiredRolls = requiredRolls,
                    DispatchedRolls = dispatchedRolls,
                    StockRolls = stockRolls,
                    UpdatedNoOfRolls = updatedNoOfRolls,
                    UpdateQuantity = updateQuantity,
                    BalanceNoOfRolls = requiredRolls - updatedNoOfRolls,
                    BalanceQuantity = pa.ActualQuantity - updateQuantity,
                    AllocatedRolls = requiredRolls,
                    AvailableQuantity = availableQuantity,
                    AvailableWeight = availableWeight,
                    CreatedDate = pa.CreatedDate
                };

                reportData.Add(dto);
            }

            return reportData.OrderByDescending(r => r.CreatedDate).ToList();
        }

        public async Task<FinalFabricReportDto> GetFinalFabricReportBySalesOrderAsync(int salesOrderId)
        {
            // For a single sales order, just fetch the list and pick the first
            var reports = await GetFinalFabricReportAsync();
            return reports.FirstOrDefault(r => r.SalesOrderId == salesOrderId);
        }
    }
}