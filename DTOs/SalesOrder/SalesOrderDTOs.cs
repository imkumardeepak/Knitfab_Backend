using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.SalesOrder
{
    // Response DTO for SalesOrder
    public class SalesOrderResponseDto
    {
        public int Id { get; set; }
        public string VchType { get; set; } = string.Empty;
        public DateTime SalesDate { get; set; }
        public string Guid { get; set; } = string.Empty;
        public string GstRegistrationType { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
        public string PartyName { get; set; } = string.Empty;
        public string PartyLedgerName { get; set; } = string.Empty;
        public string VoucherNumber { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string BuyerAddress { get; set; } = string.Empty;
        public string OrderTerms { get; set; } = string.Empty;
        public string LedgerEntries { get; set; } = string.Empty;
        public int ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public ICollection<SalesOrderItemResponseDto> Items { get; set; } = new List<SalesOrderItemResponseDto>();
    }

    // Response DTO for SalesOrderItem
    public class SalesOrderItemResponseDto
    {
        public int Id { get; set; }
        public int SalesOrderId { get; set; }
        public string StockItemName { get; set; } = string.Empty;
        public string Rate { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string ActualQty { get; set; } = string.Empty;
        public string BilledQty { get; set; } = string.Empty;
        public string Descriptions { get; set; } = string.Empty;
        public string BatchName { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public string OrderDueDate { get; set; } = string.Empty;
        public int ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
    }

    // Request DTO for creating SalesOrder
    public class CreateSalesOrderRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string VchType { get; set; } = string.Empty;

        [Required]
        public DateTime SalesDate { get; set; }

        [MaxLength(100)]
        public string Guid { get; set; } = string.Empty;

        [MaxLength(50)]
        public string GstRegistrationType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string StateName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PartyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PartyLedgerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VoucherNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Reference { get; set; } = string.Empty;

        [MaxLength(500)]
        public string CompanyAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string BuyerAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OrderTerms { get; set; } = string.Empty;

        [MaxLength(500)]
        public string LedgerEntries { get; set; } = string.Empty;

        public int ProcessFlag { get; set; } = 0;

        public DateTime ProcessDate { get; set; } = DateTime.UtcNow;

        public ICollection<CreateSalesOrderItemRequestDto> Items { get; set; } = new List<CreateSalesOrderItemRequestDto>();
    }

    // Request DTO for creating SalesOrderItem
    public class CreateSalesOrderItemRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string StockItemName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Rate { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Amount { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ActualQty { get; set; } = string.Empty;

        [MaxLength(50)]
        public string BilledQty { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Descriptions { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BatchName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string OrderNo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string OrderDueDate { get; set; } = string.Empty;
        public int ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
    }

    // Request DTO for updating SalesOrder
    public class UpdateSalesOrderRequestDto
    {
        [Required]
        [MaxLength(50)]
        public string VchType { get; set; } = string.Empty;

        [Required]
        public DateTime SalesDate { get; set; }

        [MaxLength(100)]
        public string Guid { get; set; } = string.Empty;

        [MaxLength(50)]
        public string GstRegistrationType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string StateName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PartyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PartyLedgerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string VoucherNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Reference { get; set; } = string.Empty;

        [MaxLength(500)]
        public string CompanyAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string BuyerAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string OrderTerms { get; set; } = string.Empty;

        [MaxLength(500)]
        public string LedgerEntries { get; set; } = string.Empty;

        public int ProcessFlag { get; set; }

        public DateTime ProcessDate { get; set; }

        public ICollection<UpdateSalesOrderItemRequestDto> Items { get; set; } = new List<UpdateSalesOrderItemRequestDto>();
    }

    // Request DTO for updating SalesOrderItem
    public class UpdateSalesOrderItemRequestDto
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string StockItemName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Rate { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Amount { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ActualQty { get; set; } = string.Empty;

        [MaxLength(50)]
        public string BilledQty { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Descriptions { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BatchName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string OrderNo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string OrderDueDate { get; set; } = string.Empty;
        public int ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
    }

    // Request DTO for searching SalesOrders
    public class SalesOrderSearchRequestDto
    {
        public string? VoucherNumber { get; set; }
        public string? PartyName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ProcessFlag { get; set; }
    }
}