using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AvyyanBackend.Models
{
    public class SalesOrderWeb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Voucher details
        [Required]
        [MaxLength(50)]
        public string VoucherType { get; set; } = "Sales Order"; // Default value

        [Required]
        [MaxLength(50)]
        public string VoucherNumber { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? TermsOfPayment { get; set; } = string.Empty;

        public bool IsJobWork { get; set; } = false; // Checkbox for job work

        // Serial number field
        [MaxLength(50)]
        public string? SerialNo { get; set; }

        // Additional fields
        public bool IsProcess { get; set; } = false; // Process flag

        [MaxLength(100)]
        public string? OrderNo { get; set; } = string.Empty; // Order number

        [MaxLength(200)]
        public string? TermsOfDelivery { get; set; } = string.Empty; // Terms of delivery

        [MaxLength(200)]
        public string? DispatchThrough { get; set; } = string.Empty; // Dispatch through

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

        // Other Reference field
        [MaxLength(500)]
        public string? OtherReference { get; set; } = string.Empty;

        // New fields for totals
        public decimal TotalQuantity { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;


        // Process date - Added to match SalesOrder
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? ProcessDate { get; set; }

        // Audit fields
        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;

        [Column(TypeName = "timestamp without time zone")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = string.Empty;

        // Navigation property for items
        public virtual ICollection<SalesOrderItemWeb> Items { get; set; } = new List<SalesOrderItemWeb>();
    }

    public class SalesOrderItemWeb
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SalesOrderWebId { get; set; }

        // Item details
        [MaxLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [StringLength(50)]
        public string HSNCode { get; set; } = string.Empty;


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

        [MaxLength(50)]
        public string? Unit { get; set; } // Unit field

        // New fields
        [MaxLength(50)]
        public string? SlitLine { get; set; }

        [MaxLength(50)]
        public string? StitchLength { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? DueDate { get; set; }

        // Process flag
        public bool IsProcess { get; set; } = false;


        // Process date - Added to match SalesOrderItem
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? ProcessDate { get; set; }

        // Navigation property
        [ForeignKey("SalesOrderWebId")]
        public virtual SalesOrderWeb SalesOrderWeb { get; set; } = null!;
    }
}