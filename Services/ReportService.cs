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
            var salesOrders = await _context.SalesOrdersWeb
                .Include(so => so.Items)
                .ToListAsync();

            var reports = new List<FinalFabricReportDto>();

            foreach (var salesOrder in salesOrders)
            {
                var report = await GetFinalFabricReportBySalesOrderAsync(salesOrder.Id);
                reports.Add(report);
            }

            return reports;
        }

        public async Task<FinalFabricReportDto> GetFinalFabricReportBySalesOrderAsync(int salesOrderId)
        {
            var salesOrder = await _context.SalesOrdersWeb
                .Include(so => so.Items)
                .FirstOrDefaultAsync(so => so.Id == salesOrderId);

            if (salesOrder == null)
                return null;

            var report = new FinalFabricReportDto
            {
                SalesOrderId = salesOrder.Id,
                VoucherNumber = salesOrder.VoucherNumber,
                BuyerName = salesOrder.BuyerName,
                OrderDate = salesOrder.OrderDate
            };

            // Process each sales order item
            foreach (var item in salesOrder.Items)
            {
                var itemReport = new SalesOrderItemReportDto
                {
                    SalesOrderItemId = item.Id,
                    ItemName = item.ItemName,
                    YarnCount = item.YarnCount,
                    Dia = item.Dia,
                    GG = item.GG,
                    FabricType = item.FabricType,
                    Qty = item.Qty
                };

                // Get production allotments for this sales order item
                var productionAllotments = await _context.ProductionAllotments
                    .Where(pa => pa.SalesOrderId == salesOrderId && pa.SalesOrderItemId == item.Id)
                    .ToListAsync();

                foreach (var pa in productionAllotments)
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
                        ActualQuantity = pa.ActualQuantity
                    };

                    // Get machine allocations for this production allotment
                    var machineAllocations = await _context.MachineAllocations
                        .Where(ma => ma.ProductionAllotmentId == pa.Id)
                        .ToListAsync();

                    paReport.TotalRunningMachines = machineAllocations.Count;
                    
                    foreach (var ma in machineAllocations)
                    {
                        paReport.MachineAllocations.Add(new MachineAllocationReportDto
                        {
                            MachineAllocationId = ma.Id,
                            MachineName = ma.MachineName,
                            NumberOfNeedles = ma.NumberOfNeedles,
                            Feeders = ma.Feeders,
                            RPM = ma.RPM,
                            TotalRolls = ma.TotalRolls
                        });
                    }

                    // Get roll confirmations for this production allotment (using AllotId)
                    var rollConfirmations = await _context.RollConfirmations
                        .Where(rc => rc.AllotId == pa.AllotmentId)
                        .ToListAsync();

                    paReport.TotalConfirmedRolls = rollConfirmations.Count;
                    paReport.TotalConfirmedNetWeight = rollConfirmations.Sum(rc => rc.NetWeight ?? 0);

                    foreach (var rc in rollConfirmations)
                    {
                        paReport.RollConfirmations.Add(new RollConfirmationReportDto
                        {
                            RollConfirmationId = rc.Id,
                            RollNo = rc.RollNo,
                            MachineName = rc.MachineName,
                            NetWeight = rc.NetWeight ?? 0,
                            GreyGsm = rc.GreyGsm,
                            GreyWidth = rc.GreyWidth
                        });
                    }

                    // Get dispatch planning for this production allotment (using YarnLotNo which matches LotNo)
                    var dispatchPlannings = await _context.DispatchPlannings
                        .Where(dp => dp.LotNo == pa.YarnLotNo)
                        .ToListAsync();

                    paReport.TotalDispatchedRolls = (int)dispatchPlannings.Sum(dp => dp.TotalDispatchedRolls);
                    paReport.TotalDispatchedNetWeight = dispatchPlannings.Sum(dp => dp.TotalNetWeight ?? 0);

                    foreach (var dp in dispatchPlannings)
                    {
                        paReport.DispatchPlannings.Add(new DispatchPlanningReportDto
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
                        });
                    }

                    itemReport.ProductionAllotments.Add(paReport);
                }

                report.SalesOrderItems.Add(itemReport);
            }

            return report;
        }
    }
}