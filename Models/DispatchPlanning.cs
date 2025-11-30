﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class DispatchPlanning : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string LotNo { get; set; } = string.Empty;

        public int SalesOrderId { get; set; }

        public int SalesOrderItemId { get; set; }

        [Required]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Tape { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,3)")]
        public decimal TotalRequiredRolls { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal TotalReadyRolls { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal TotalDispatchedRolls { get; set; }

        public bool IsFullyDispatched { get; set; }

        public DateTime? DispatchStartDate { get; set; }

        public DateTime? DispatchEndDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string VehicleNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DriverName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string License { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        public string Remarks { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string LoadingNo { get; set; } = string.Empty;

        [MaxLength(20)]
        public string DispatchOrderId { get; set; } = string.Empty;

        // Transport/Courier foreign key fields
        public bool IsTransport { get; set; } = false;
        public bool IsCourier { get; set; } = false;
        public int? TransportId { get; set; }
        public int? CourierId { get; set; }

        // Manual transport details (new fields)
        [MaxLength(200)]
        public string TransportName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string ContactPerson { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,3)")]
        public decimal? MaximumCapacityKgs { get; set; }

        // Weight fields for dispatch planning
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalGrossWeight { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalNetWeight { get; set; }

        // Navigation properties
        [ForeignKey("TransportId")]
        public virtual TransportMaster? Transport { get; set; }

        [ForeignKey("CourierId")]
        public virtual CourierMaster? Courier { get; set; }
    }
}