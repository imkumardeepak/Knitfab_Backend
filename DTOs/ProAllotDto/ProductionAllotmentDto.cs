﻿using AvyyanBackend.Models.ProAllot;
using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.ProAllotDto
{
	public class CreateProductionAllotmentRequest
	{
		public string AllotmentId { get; set; }
		public string VoucherNumber { get; set; }
		public string ItemName { get; set; }
		public int SalesOrderId { get; set; }
		public int SalesOrderItemId { get; set; }
		public decimal ActualQuantity { get; set; }
		public string YarnCount { get; set; }
		public int Diameter { get; set; }
		public int Gauge { get; set; }
		public string FabricType { get; set; }
		public string SlitLine { get; set; }
		public string StitchLength { get; set; }
		public decimal Efficiency { get; set; }
		public string Composition { get; set; }
		public string YarnLotNo { get; set; }
		public string Counter { get; set; }
		public string ColourCode { get; set; }
		public decimal? ReqGreyGsm { get; set; }
		public decimal? ReqGreyWidth { get; set; }
		public decimal? ReqFinishGsm { get; set; }
		public decimal? ReqFinishWidth { get; set; }
		public string YarnPartyName { get; set; } // New field for yarn party name
		public string PolybagColor { get; set; } // New field for polybag color
		public string PartyName { get; set; }

		// Other Reference field
		[MaxLength(500)]
		public string? OtherReference { get; set; }

		// Packaging Details
		public decimal TubeWeight { get; set; }
		public decimal? ShrinkRapWeight { get; set; }
		public decimal? TotalWeight { get; set; }
		public string TapeColor { get; set; }

		public List<MachineAllocationRequest> MachineAllocations { get; set; }
	}

	public class MachineAllocationRequest
	{
		public int? Id { get; set; } // Optional ID for updates
		public string MachineName { get; set; }
		public int MachineId { get; set; }
		public int NumberOfNeedles { get; set; }
		public int Feeders { get; set; }
		public decimal RPM { get; set; }
		public decimal RollPerKg { get; set; }
		public decimal TotalLoadWeight { get; set; }
		public decimal TotalRolls { get; set; }
		public RollBreakdown RollBreakdown { get; set; }
		public decimal EstimatedProductionTime { get; set; }
	}

	// New DTO for updating machine allocations
	public class UpdateMachineAllocationsRequest
	{
		public List<MachineAllocationRequest> MachineAllocations { get; set; }
	}

	public class RollBreakdown
	{
		public List<RollItem> WholeRolls { get; set; } = new List<RollItem>();
		public RollItem FractionalRoll { get; set; }
	}

	public class RollItem
	{
		public int Quantity { get; set; }
		public decimal WeightPerRoll { get; set; }
		public decimal TotalWeight { get; set; }
	}

	// Response DTOs
	public class ProductionAllotmentResponseDto
	{
		public int Id { get; set; }
		public string AllotmentId { get; set; }
		public string VoucherNumber { get; set; }
		public string ItemName { get; set; }
		public int SalesOrderId { get; set; }
		public int SalesOrderItemId { get; set; }
		public decimal ActualQuantity { get; set; }
		public string YarnCount { get; set; }
		public int Diameter { get; set; }
		public int Gauge { get; set; }
		public string FabricType { get; set; }
		public string SlitLine { get; set; }
		public string StitchLength { get; set; }
		public decimal Efficiency { get; set; }
		public string Composition { get; set; }
		public decimal TotalProductionTime { get; set; }
		public DateTime CreatedDate { get; set; }
		public string YarnLotNo { get; set; }
		public string Counter { get; set; }
		public string ColourCode { get; set; }
		public decimal? ReqGreyGsm { get; set; }
		public decimal? ReqGreyWidth { get; set; }
		public decimal? ReqFinishGsm { get; set; }
		public decimal? ReqFinishWidth { get; set; }
		public string YarnPartyName { get; set; } // New field for yarn party name
		public string PolybagColor { get; set; } // New field for polybag color
		public string PartyName { get; set; }

		// Other Reference field
		[MaxLength(500)]
		public string? OtherReference { get; set; }

		// Packaging Details
		public decimal TubeWeight { get; set; }
		public decimal? ShrinkRapWeight { get; set; }
		public decimal? TotalWeight { get; set; }
		public string TapeColor { get; set; }

		public string SerialNo { get; set; } // New Serial Number field
		public bool IsOnHold { get; set; } // Flag to indicate if production is on hold
		public bool IsSuspended { get; set; } // Flag to indicate if production is suspended
		public List<MachineAllocationResponseDto> MachineAllocations { get; set; } = new List<MachineAllocationResponseDto>();
	}

	public class MachineAllocationResponseDto
	{
		public int Id { get; set; }
		public int ProductionAllotmentId { get; set; }
		public string MachineName { get; set; }
		public int MachineId { get; set; }
		public int NumberOfNeedles { get; set; }
		public int Feeders { get; set; }
		public decimal RPM { get; set; }
		public decimal RollPerKg { get; set; }
		public decimal TotalLoadWeight { get; set; }
		public decimal TotalRolls { get; set; }
		public RollBreakdown RollBreakdown { get; set; }
		public decimal EstimatedProductionTime { get; set; }
	}
}