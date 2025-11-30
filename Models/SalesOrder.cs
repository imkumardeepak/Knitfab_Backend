using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TallyERPWebApi.Model;

namespace AvyyanBackend.Models
{
	public class SalesOrder:BaseEntity
	{
		public int Id { get; set; }


		[MaxLength(50)]
		public string VchType { get; set; }

		// Voucher details
		[MaxLength(20)]
		[Column(TypeName = "timestamp without time zone")]
		public DateTime SalesDate { get; set; }

		[MaxLength(100)]
		public string Guid { get; set; }

		[MaxLength(50)]
		public string GstRegistrationType { get; set; }

		[MaxLength(100)]
		public string StateName { get; set; }

		[MaxLength(200)]
		public string PartyName { get; set; }

		[MaxLength(200)]
		public string PartyLedgerName { get; set; }

		[MaxLength(50)]
		public string VoucherNumber { get; set; }

		[MaxLength(100)]
		public string Reference { get; set; }

		// Address information (combined into single fields)
		[MaxLength(500)]
		public string CompanyAddress { get; set; }

		[MaxLength(500)]
		public string BuyerAddress { get; set; }

		[MaxLength(500)]
		public string OrderTerms { get; set; }

		// Ledger information (combined)
		[MaxLength(500)]
		public string LedgerEntries { get; set; }

		public int ProcessFlag { get; set; } = 0;

        [Column(TypeName = "timestamp without time zone")]
		public DateTime ProcessDate { get; set; } = DateTime.Now;

		public virtual ICollection<SalesOrderItem> Items { get; set; }
	}

	public class SalesOrderItem
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int SalesOrderId { get; set; }

		// Item basic information
		[MaxLength(200)]
		public string StockItemName { get; set; }

		[MaxLength(50)]
		public string Rate { get; set; }

		[MaxLength(50)]
		public string Amount { get; set; }

		[MaxLength(50)]
		public string ActualQty { get; set; }

		[MaxLength(50)]
		public string BilledQty { get; set; }

		// Item descriptions (combined into single field)
		[MaxLength(2000)]
		public string Descriptions { get; set; }

		// Batch information (if needed)
		[MaxLength(100)]
		public string BatchName { get; set; }

		[MaxLength(50)]
		public string OrderNo { get; set; }

		[MaxLength(50)]
		public string OrderDueDate { get; set; }
		public int ProcessFlag { get; set; } = 0;

		[Column(TypeName = "timestamp without time zone")]
		public DateTime ProcessDate { get; set; } = DateTime.Now;

		[ForeignKey("SalesOrderId")]
		[JsonIgnore]
		public virtual SalesOrder Voucher { get; set; }
	}
}
