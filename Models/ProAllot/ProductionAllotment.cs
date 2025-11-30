using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models.ProAllot
{


	public class ProductionAllotment
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string AllotmentId { get; set; } // Generated using your format

		[Required]
		[MaxLength(50)]
		public string VoucherNumber { get; set; }

		[Required]
		[MaxLength(200)]
		public string ItemName { get; set; }

		public int SalesOrderId { get; set; }
		public int SalesOrderItemId { get; set; }

		[Column(TypeName = "decimal(18,3)")]
		public decimal ActualQuantity { get; set; }

		[MaxLength(50)]
		public string YarnCount { get; set; } // e.g., "24/1 CCH"

		public int Diameter { get; set; }
		public int Gauge { get; set; }

		[MaxLength(100)]
		public string FabricType { get; set; }

		[MaxLength(200)]
		public string SlitLine { get; set; }

		[MaxLength(200)]
		public string StitchLength { get; set; }

		[Column(TypeName = "decimal(5,2)")]
		public decimal Efficiency { get; set; }

		[MaxLength(200)]
		public string Composition { get; set; } // e.g., "97% Cotton + 3% Lycra"

		[Column(TypeName = "decimal(18,2)")]
		public decimal TotalProductionTime { get; set; } // In days

		[MaxLength(50)]
		public string YarnLotNo { get; set; }

		[MaxLength(50)]
		public string Counter { get; set; }

		[MaxLength(50)]
		public string ColourCode { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal? ReqGreyGsm { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal? ReqGreyWidth { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal? ReqFinishGsm { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal? ReqFinishWidth { get; set; }

		[MaxLength(200)]
		public string PartyName { get; set; }

		// Packaging Details
		[Column(TypeName = "decimal(18,3)")]
		public decimal TubeWeight { get; set; }

		[Column(TypeName = "decimal(18,3)")]
		public decimal? ShrinkRapWeight { get; set; }

		[Column(TypeName = "decimal(18,3)")]
		public decimal? TotalWeight { get; set; }

		[MaxLength(100)]
		public string TapeColor { get; set; }

		public string? SerialNo { get; set; } // Format: "0001", "0002", etc."

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		// Navigation property
		public virtual ICollection<MachineAllocation> MachineAllocations { get; set; }
	}

	public class MachineAllocation
	{
		public int Id { get; set; }
		public int ProductionAllotmentId { get; set; }

		[Required]
		[MaxLength(100)]
		public string MachineName { get; set; }

		public int MachineId { get; set; } // Reference to the machine table

		public int NumberOfNeedles { get; set; }
		public int Feeders { get; set; }
		public decimal RPM { get; set; }

		[Column(TypeName = "decimal(18,3)")]
		public decimal RollPerKg { get; set; }

		[Column(TypeName = "decimal(18,3)")]
		public decimal TotalLoadWeight { get; set; }

		[Column(TypeName = "decimal(18,3)")]
		public decimal TotalRolls { get; set; }

		// Store roll breakdown as JSON for flexibility
		public string RollBreakdown { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal EstimatedProductionTime { get; set; } // In days

		// Navigation property
		public virtual ProductionAllotment ProductionAllotment { get; set; }

		// Navigation property for roll assignments
		public virtual ICollection<RollAssignment> RollAssignments { get; set; } = new List<RollAssignment>();
	}
}