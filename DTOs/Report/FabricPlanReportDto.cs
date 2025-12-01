using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.Report
{
    public class FabricPlanReportDto
    {
        public string DiaGg { get; set; }
        public DateTime ProgramCompletionDate { get; set; }
        public string CustomerName { get; set; }
        public int Count { get; set; }
        public string FabricLotNo { get; set; }
        public int NumberOfRunningMachines { get; set; }
        public decimal SumOfPerDayProduction { get; set; }
        public decimal SumOfOrderQuantity { get; set; }
        public decimal SumOfUpdatedQuantity { get; set; }
        public decimal SumOfBalanceQuantity { get; set; }
        public int BalanceDays { get; set; }
    }

    public class FabricPlanReportFilterDto
    {
        public string DiaGg { get; set; }
        public string CustomerName { get; set; }
        public string YarnCount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class FabricPlanReportSummaryDto
    {
        public string DiaGg { get; set; }
        public decimal TotalPerDayProduction { get; set; }
        public decimal TotalOrderQuantity { get; set; }
        public decimal TotalUpdatedQuantity { get; set; }
        public decimal TotalBalanceQuantity { get; set; }
        public int TotalRunningMachines { get; set; }
    }

    public class FabricPlanReportResponseDto
    {
        public List<FabricPlanReportDto> Reports { get; set; }
        public List<FabricPlanReportSummaryDto> Summaries { get; set; }
        public FabricPlanReportSummaryDto GrandTotal { get; set; }
    }
    
    public class FabricPlanFilterOptionsDto
    {
        public List<string> DiaGgOptions { get; set; }
        public List<string> CustomerOptions { get; set; }
        public List<string> YarnCountOptions { get; set; }
    }
}