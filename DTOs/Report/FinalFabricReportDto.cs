using System;
using System.Collections.Generic;

namespace AvyyanBackend.DTOs.Report
{
    public class FinalFabricReportDto
    {
        // Sales Order Information
        public int SalesOrderId { get; set; }
        public string VoucherNumber { get; set; }
        public string BuyerName { get; set; }
        public DateTime OrderDate { get; set; }
        
        // Sales Order Item Information
        public List<SalesOrderItemReportDto> SalesOrderItems { get; set; } = new List<SalesOrderItemReportDto>();
    }

    public class SalesOrderItemReportDto
    {
        public int SalesOrderItemId { get; set; }
        public string ItemName { get; set; }
        public string YarnCount { get; set; }
        public int Dia { get; set; }
        public int GG { get; set; }
        public string FabricType { get; set; }
        public decimal Qty { get; set; }
        
        // Production Allotment Information
        public List<ProductionAllotmentReportDto> ProductionAllotments { get; set; } = new List<ProductionAllotmentReportDto>();
    }

    public class ProductionAllotmentReportDto
    {
        public int ProductionAllotmentId { get; set; }
        public string AllotmentId { get; set; }
        public string YarnCount { get; set; }
        public int Diameter { get; set; }
        public int Gauge { get; set; }
        public string FabricType { get; set; }
        public string PartyName { get; set; }
        public string YarnPartyName { get; set; }
        public string YarnLotNo { get; set; }
        public decimal ActualQuantity { get; set; }
        
        // Machine Allocation Information
        public int TotalRunningMachines { get; set; }
        public List<MachineAllocationReportDto> MachineAllocations { get; set; } = new List<MachineAllocationReportDto>();
        
        // Roll Confirmation Information
        public List<RollConfirmationReportDto> RollConfirmations { get; set; } = new List<RollConfirmationReportDto>();
        public decimal TotalConfirmedNetWeight { get; set; }
        public int TotalConfirmedRolls { get; set; }
        
        // Dispatch Information
        public List<DispatchPlanningReportDto> DispatchPlannings { get; set; } = new List<DispatchPlanningReportDto>();
        public decimal TotalDispatchedNetWeight { get; set; }
        public int TotalDispatchedRolls { get; set; }
    }

    public class MachineAllocationReportDto
    {
        public int MachineAllocationId { get; set; }
        public string MachineName { get; set; }
        public int NumberOfNeedles { get; set; }
        public int Feeders { get; set; }
        public decimal RPM { get; set; }
        public decimal TotalRolls { get; set; }
    }

    public class RollConfirmationReportDto
    {
        public int RollConfirmationId { get; set; }
        public string RollNo { get; set; }
        public string MachineName { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GreyGsm { get; set; }
        public decimal GreyWidth { get; set; }
    }

    public class DispatchPlanningReportDto
    {
        public int DispatchPlanningId { get; set; }
        public string LotNo { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalRequiredRolls { get; set; }
        public decimal TotalReadyRolls { get; set; }
        public decimal TotalDispatchedRolls { get; set; }
        public decimal? TotalNetWeight { get; set; }
        public string VehicleNo { get; set; }
        public DateTime? DispatchStartDate { get; set; }
        public DateTime? DispatchEndDate { get; set; }
    }
}