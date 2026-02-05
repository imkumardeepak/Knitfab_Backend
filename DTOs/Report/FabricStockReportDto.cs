using System;

namespace AvyyanBackend.DTOs.Report
{
    public class FabricStockReportDto
    {
        public string LotNo { get; set; }
        public string VoucherNumber { get; set; }
        public string ItemName { get; set; }
        public string CustomerName { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal RequiredRolls { get; set; }
        public int DispatchedRolls { get; set; }
        public int StockRolls { get; set; }
        public int UpdatedNoOfRolls { get; set; }
        public decimal UpdateQuantity { get; set; }
        public decimal BalanceNoOfRolls { get; set; }
        public decimal BalanceQuantity { get; set; }
        public decimal AllocatedRolls { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
