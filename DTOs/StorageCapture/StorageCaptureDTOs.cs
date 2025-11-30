using AvyyanBackend.DTOs.ProductionConfirmation;
using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.StorageCapture
{
	/// <summary>
	/// DTO for StorageCapture data response
	/// </summary>
	public class StorageCaptureResponseDto
	{
		public int Id { get; set; }
		public string LotNo { get; set; } = string.Empty;
		public string FGRollNo { get; set; } = string.Empty;
		public string LocationCode { get; set; } = string.Empty;
		public string Tape { get; set; } = string.Empty;
		public string CustomerName { get; set; } = string.Empty;
		public bool IsDispatched { get; set; } = false;
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public bool IsActive { get; set; }
	}

	/// <summary>
	/// DTO for creating a new StorageCapture record
	/// </summary>
	public class CreateStorageCaptureRequestDto
	{
		[Required]
		[MaxLength(100)]
		public string LotNo { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		public string FGRollNo { get; set; } = string.Empty;

		[Required]
		[MaxLength(50)]
		public string LocationCode { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		public string Tape { get; set; } = string.Empty;

		[Required]
		[MaxLength(200)]
		public string CustomerName { get; set; } = string.Empty;
		
		public bool IsDispatched { get; set; } = false;
	}

	/// <summary>
	/// DTO for updating an existing StorageCapture record
	/// </summary>
	public class UpdateStorageCaptureRequestDto
	{
		[Required]
		[MaxLength(100)]
		public string LotNo { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		public string FGRollNo { get; set; } = string.Empty;

		[Required]
		[MaxLength(50)]
		public string LocationCode { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		public string Tape { get; set; } = string.Empty;

		[Required]
		[MaxLength(200)]
		public string CustomerName { get; set; } = string.Empty;

		public bool IsDispatched { get; set; } = false;
		public bool IsActive { get; set; } = true;
	}

	/// <summary>
	/// DTO for StorageCapture search request
	/// </summary>
	public class StorageCaptureSearchRequestDto
	{
		[MaxLength(100)]
		public string? LotNo { get; set; }

		[MaxLength(100)]
		public string? FGRollNo { get; set; }

		[MaxLength(50)]
		public string? LocationCode { get; set; }

		[MaxLength(100)]
		public string? Tape { get; set; }

		[MaxLength(200)]
		public string? CustomerName { get; set; }

		public bool? IsActive { get; set; }
		public bool? IsDispatched { get; set; }
	}

	/// <summary>
	/// DTO for combined response including roll confirmations, machine allocations, and production allotment
	/// </summary>
	public class StorageCaptureRollDataResponseDto
	{
		public RollConfirmationResponseDto RollConfirmation { get; set; } = new RollConfirmationResponseDto();
		public MachineAllocationDto MachineAllocation { get; set; } = new MachineAllocationDto();
		public ProductionAllotmentDto ProductionAllotment { get; set; } = new ProductionAllotmentDto();
	}

	/// <summary>
	/// DTO for Machine Allocation data
	/// </summary>
	public class MachineAllocationDto
	{
		public int Id { get; set; }
		public int ProductionAllotmentId { get; set; }
		public string MachineName { get; set; } = string.Empty;
		public int MachineId { get; set; }
		public int NumberOfNeedles { get; set; }
		public int Feeders { get; set; }
		public decimal RPM { get; set; }
		public decimal RollPerKg { get; set; }
		public decimal TotalLoadWeight { get; set; }
		public decimal TotalRolls { get; set; }
		public string RollBreakdown { get; set; } = string.Empty;
		public decimal EstimatedProductionTime { get; set; }
	}

	/// <summary>
	/// DTO for Production Allotment data
	/// </summary>
	public class ProductionAllotmentDto
	{
		public int Id { get; set; }
		public string AllotmentId { get; set; } = string.Empty;
		public string VoucherNumber { get; set; } = string.Empty;
		public string ItemName { get; set; } = string.Empty;
		public int SalesOrderId { get; set; }
		public int SalesOrderItemId { get; set; }
		public decimal ActualQuantity { get; set; }
		public string YarnCount { get; set; } = string.Empty;
		public int Diameter { get; set; }
		public int Gauge { get; set; }
		public string FabricType { get; set; } = string.Empty;
		public string SlitLine { get; set; } = string.Empty;
		public decimal StitchLength { get; set; }
		public decimal Efficiency { get; set; }
		public string Composition { get; set; } = string.Empty;
		public decimal TotalProductionTime { get; set; }
		public DateTime CreatedDate { get; set; }
		public string YarnLotNo { get; set; } = string.Empty;
		public string Counter { get; set; } = string.Empty;
		public string ColourCode { get; set; } = string.Empty;
		public decimal? ReqGreyGsm { get; set; }
		public decimal? ReqGreyWidth { get; set; }
		public decimal? ReqFinishGsm { get; set; }
		public decimal? ReqFinishWidth { get; set; }
		public string PartyName { get; set; } = string.Empty;
		public decimal TubeWeight { get; set; }
		public string TapeColor { get; set; } = string.Empty;
	}
}