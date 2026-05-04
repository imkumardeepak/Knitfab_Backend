namespace AvyyanBackend.DTOs.Report
{
    public class DispatchReportDto
    {
        public string LoadingSheetNo { get; set; } = string.Empty;
        public string DispatchOrderId { get; set; } = string.Empty;
        public string Voucher { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Lots { get; set; } = string.Empty;
        public DateTime? DispatchDate { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
    }
}
