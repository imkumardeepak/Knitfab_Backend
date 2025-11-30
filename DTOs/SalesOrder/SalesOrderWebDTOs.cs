using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.SalesOrder
{
    // Response DTO for SalesOrderWeb
    public class SalesOrderWebResponseDto
    {
        public int Id { get; set; }

        // Voucher details
        public string VoucherType { get; set; } = "Sales Order"; // Default value
        public string VoucherNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string TermsOfPayment { get; set; } = string.Empty;
        public bool IsJobWork { get; set; } = false; // Checkbox for job work
        
        // Serial number field
        public string? SerialNo { get; set; }

        // Company details
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyGSTIN { get; set; } = string.Empty;
        public string CompanyState { get; set; } = string.Empty;

        // Buyer details (Bill To)
        public string BuyerName { get; set; } = string.Empty;
        public string? BuyerGSTIN { get; set; } // Made nullable
        public string? BuyerState { get; set; } // Made nullable
        public string BuyerPhone { get; set; } = string.Empty;
        public string BuyerContactPerson { get; set; } = string.Empty;
        public string BuyerAddress { get; set; } = string.Empty;

        // Consignee details (Ship To)
        public string ConsigneeName { get; set; } = string.Empty;
        public string? ConsigneeGSTIN { get; set; } // Made nullable
        public string? ConsigneeState { get; set; } // Made nullable
        public string ConsigneePhone { get; set; } = string.Empty;
        public string ConsigneeContactPerson { get; set; } = string.Empty;
        public string ConsigneeAddress { get; set; } = string.Empty;

        // Order details
        public string Remarks { get; set; } = string.Empty;

        // New fields for totals
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }

        // Items
        public ICollection<SalesOrderItemWebResponseDto> Items { get; set; } = new List<SalesOrderItemWebResponseDto>();
        
        // Process fields
        public bool IsProcess { get; set; } = false;
        public DateTime? ProcessDate { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    // Response DTO for SalesOrderItemWeb
    public class SalesOrderItemWebResponseDto
    {
        public int Id { get; set; }
        public int SalesOrderWebId { get; set; }

        // Item details
        public string ItemName { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string YarnCount { get; set; } = string.Empty;
        public int Dia { get; set; } = 0;
        public int GG { get; set; } = 0;
        
        // Computed property for backward compatibility
        public string DiaGG => $"{Dia}*{GG}";
        
        public string FabricType { get; set; } = string.Empty;
        public string Composition { get; set; } = string.Empty;
        public decimal WtPerRoll { get; set; }
        public int NoOfRolls { get; set; }
        public decimal Rate { get; set; }
        public decimal Qty { get; set; }
        public decimal Amount { get; set; }
        public decimal IGST { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; } // Added CGST field
        public string Remarks { get; set; } = string.Empty;
        
        // New fields
        public string? SlitLine { get; set; }
        public string? StitchLength { get; set; }
        public DateTime? DueDate { get; set; }
        
        // Process fields
        public bool IsProcess { get; set; } = false;
        public DateTime? ProcessDate { get; set; }
    }

    // Request DTO for creating SalesOrderWeb
    public class CreateSalesOrderWebRequestDto
    {
        // Voucher details
        [Required]
        [MaxLength(50)]
        public string VoucherType { get; set; } = "Sales Order"; // Default value

        [Required]
        [MaxLength(50)]
        public string VoucherNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string TermsOfPayment { get; set; } = string.Empty;

        public bool IsJobWork { get; set; } = false; // Checkbox for job work
        
        // Serial number field
        [MaxLength(50)]
        public string? SerialNo { get; set; }

        // Company details
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CompanyGSTIN { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string CompanyState { get; set; } = string.Empty;

        // Buyer details (Bill To)
        [Required]
        [MaxLength(200)]
        public string BuyerName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? BuyerGSTIN { get; set; } // Made nullable

        [MaxLength(100)]
        public string? BuyerState { get; set; } // Made nullable

        [MaxLength(20)]
        public string BuyerPhone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BuyerContactPerson { get; set; } = string.Empty;

        [MaxLength(200)]
        public string BuyerAddress { get; set; } = string.Empty;

        // Consignee details (Ship To)
        [Required]
        [MaxLength(200)]
        public string ConsigneeName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ConsigneeGSTIN { get; set; } // Made nullable

        [MaxLength(100)]
        public string? ConsigneeState { get; set; } // Made nullable

        [MaxLength(20)]
        public string ConsigneePhone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ConsigneeContactPerson { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ConsigneeAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        // New fields for totals
        public decimal TotalQuantity { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;

        // Items
        public ICollection<CreateSalesOrderItemWebRequestDto> Items { get; set; } = new List<CreateSalesOrderItemWebRequestDto>();
    }

    // Request DTO for creating SalesOrderItemWeb
    public class CreateSalesOrderItemWebRequestDto
    {
        // Item details
        [Required]
        [MaxLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ItemDescription { get; set; } = string.Empty;

        [MaxLength(50)]
        public string YarnCount { get; set; } = string.Empty;

        public int Dia { get; set; } = 0;
        public int GG { get; set; } = 0;

        [MaxLength(100)]
        public string FabricType { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Composition { get; set; } = string.Empty;

        public decimal WtPerRoll { get; set; }

        public int NoOfRolls { get; set; }

        public decimal Rate { get; set; }

        public decimal Qty { get; set; }

        public decimal Amount { get; set; }

        public decimal IGST { get; set; }

        public decimal SGST { get; set; }

        public decimal CGST { get; set; } // Added CGST field

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;
        
        // New fields
        [MaxLength(50)]
        public string? SlitLine { get; set; }
        
        [MaxLength(50)]
        public string? StitchLength { get; set; }
        
        public DateTime? DueDate { get; set; }
    }

    // Request DTO for updating SalesOrderWeb
    public class UpdateSalesOrderWebRequestDto
    {
        // Voucher details
        [Required]
        [MaxLength(50)]
        public string VoucherType { get; set; } = "Sales Order"; // Default value

        [Required]
        [MaxLength(50)]
        public string VoucherNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        [MaxLength(50)]
        public string TermsOfPayment { get; set; } = string.Empty;

        public bool IsJobWork { get; set; } = false; // Checkbox for job work
        
        // Serial number field
        [MaxLength(50)]
        public string? SerialNo { get; set; }

        // Company details
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CompanyGSTIN { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string CompanyState { get; set; } = string.Empty;

        // Buyer details (Bill To)
        [Required]
        [MaxLength(200)]
        public string BuyerName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? BuyerGSTIN { get; set; } // Made nullable

        [MaxLength(100)]
        public string? BuyerState { get; set; } // Made nullable

        [MaxLength(20)]
        public string BuyerPhone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BuyerContactPerson { get; set; } = string.Empty;

        [MaxLength(200)]
        public string BuyerAddress { get; set; } = string.Empty;

        // Consignee details (Ship To)
        [Required]
        [MaxLength(200)]
        public string ConsigneeName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ConsigneeGSTIN { get; set; } // Made nullable

        [MaxLength(100)]
        public string? ConsigneeState { get; set; } // Made nullable

        [MaxLength(20)]
        public string ConsigneePhone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ConsigneeContactPerson { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ConsigneeAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        // New fields for totals
        public decimal TotalQuantity { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;

        // Items
        public ICollection<UpdateSalesOrderItemWebRequestDto> Items { get; set; } = new List<UpdateSalesOrderItemWebRequestDto>();
    }

    // Request DTO for updating SalesOrderItemWeb
    public class UpdateSalesOrderItemWebRequestDto
    {
        public int? Id { get; set; }

        // Item details
        [Required]
        [MaxLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string ItemDescription { get; set; } = string.Empty;

        [MaxLength(50)]
        public string YarnCount { get; set; } = string.Empty;

        public int Dia { get; set; } = 0;
        public int GG { get; set; } = 0;

        [MaxLength(100)]
        public string FabricType { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Composition { get; set; } = string.Empty;

        public decimal WtPerRoll { get; set; }

        public int NoOfRolls { get; set; }

        public decimal Rate { get; set; }

        public decimal Qty { get; set; }

        public decimal Amount { get; set; }

        public decimal IGST { get; set; }

        public decimal SGST { get; set; }

        public decimal CGST { get; set; } // Added CGST field

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;
        
        // New fields
        [MaxLength(50)]
        public string? SlitLine { get; set; }
        
        [MaxLength(50)]
        public string? StitchLength { get; set; }
        
        public DateTime? DueDate { get; set; }
    }
}