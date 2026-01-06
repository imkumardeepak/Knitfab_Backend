namespace AvyyanBackend.DTOs.DispatchPlanning
{
	public class DispatchPlanningSummaryRequestDto
	{
		public List<string> VoucherNumbers { get; set; } = new();
	}

	public class DispatchPlanningSummaryResponseDto
	{
		public List<LotSummaryDto> Lots { get; set; } = new();
	}

	public class LotSummaryDto
	{
		public string LotNo { get; set; } = string.Empty;
		public string CustomerName { get; set; } = string.Empty;
		public string Tape { get; set; } = string.Empty;
		public int TotalRolls { get; set; }
		public int DispatchedRolls { get; set; }
		public decimal TotalNetWeight { get; set; }
		public decimal TotalActualQuantity { get; set; }
		public int TotalRequiredRolls { get; set; }
		public bool IsDispatched { get; set; }
		public SalesOrderSummaryDto? SalesOrder { get; set; }
		public string? SalesOrderItemName { get; set; }
		public List<RollSummaryDto> Rolls { get; set; } = new();
		public LoadingSheetSummaryDto? LoadingSheet { get; set; }
	}

	public class SalesOrderSummaryDto
	{
		public int Id { get; set; }
		public string VoucherNumber { get; set; } = string.Empty;
		public string BuyerName { get; set; } = string.Empty;
		public DateTime OrderDate { get; set; }
	}

	public class RollSummaryDto
	{
		public string FgRollNo { get; set; } = string.Empty;
		public string MachineName { get; set; } = string.Empty;
		public string RollNo { get; set; } = string.Empty;
		public decimal NetWeight { get; set; }
	}

	public class LoadingSheetSummaryDto
	{
		public int Id { get; set; }
		public string LoadingNo { get; set; } = string.Empty;
		public string DispatchOrderId { get; set; } = string.Empty;
		public string VehicleNo { get; set; } = string.Empty;
		public string DriverName { get; set; } = string.Empty;
		public int TotalDispatchedRolls { get; set; }
		public string Remarks { get; set; } = string.Empty;
	}
}
