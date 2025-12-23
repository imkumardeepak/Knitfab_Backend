using AvyyanBackend.Models;
using AvyyanBackend.Models.ProAllot;
using AvyyanBackend.Models.ProductionConfirmation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using TallyERPWebApi.Model;

namespace AvyyanBackend.Data
{
	public class ApplicationDbContext : DbContext
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null) : base(options)
		{
			_httpContextAccessor = httpContextAccessor ?? null;
		}
		public DbSet<ProductionAllotment> ProductionAllotments { get; set; }
		public DbSet<MachineAllocation> MachineAllocations { get; set; }
		public DbSet<RollAssignment> RollAssignments { get; set; }
		public DbSet<GeneratedBarcode> GeneratedBarcodes { get; set; }
		public DbSet<RollConfirmation> RollConfirmations { get; set; }
		public DbSet<Inspection> Inspections { get; set; }

		public DbSet<MachineManager> MachineManagers { get; set; }
		public DbSet<FabricStructureMaster> FabricStructureMasters { get; set; }
		public DbSet<LocationMaster> LocationMasters { get; set; }
		public DbSet<YarnTypeMaster> YarnTypeMasters { get; set; }

		public DbSet<TapeColorMaster> TapeColorMasters { get; set; }

		public DbSet<User> Users { get; set; }
		public DbSet<RoleMaster> RoleMasters { get; set; }
		public DbSet<PageAccess> PageAccesses { get; set; }


		public DbSet<ShiftMaster> ShiftMasters { get; set; }
		public DbSet<StorageCapture> StorageCaptures { get; set; }
		public DbSet<TransportMaster> TransportMasters { get; set; }
		public DbSet<CourierMaster> CourierMasters { get; set; }
		public DbSet<SlitLineMaster> SlitLineMasters { get; set; }
		
		// Dispatch Planning entities
		public DbSet<DispatchPlanning> DispatchPlannings { get; set; }
		public DbSet<DispatchedRoll> DispatchedRolls { get; set; }
		
		// Sales Order Web entities
		public DbSet<SalesOrderWeb> SalesOrdersWeb { get; set; }
		public DbSet<SalesOrderItemWeb> SalesOrderItemsWeb { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp with time zone");
                    }
                }
            }

            base.OnModelCreating(modelBuilder);

			// Configure entity relationships and constraints

			// MachineManager configurations
			modelBuilder.Entity<MachineManager>()
				.HasIndex(m => m.MachineName)
				.IsUnique();

			// Authentication configurations
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();

			modelBuilder.Entity<RoleMaster>()
				.HasIndex(r => r.RoleName)
				.IsUnique();

			modelBuilder.Entity<PageAccess>()
				.HasIndex(p => p.RoleId);

			// PageAccess relationship
			modelBuilder.Entity<PageAccess>()
				.HasOne(pa => pa.Role)
				.WithMany(r => r.PageAccesses)
				.HasForeignKey(pa => pa.RoleId)
				.OnDelete(DeleteBehavior.Cascade);



			// Configure relationships for SalesOrderWeb
			modelBuilder.Entity<SalesOrderWeb>()
				.HasMany(sow => sow.Items)
				.WithOne(soiw => soiw.SalesOrderWeb)
				.HasForeignKey(soiw => soiw.SalesOrderWebId)
				.OnDelete(DeleteBehavior.Cascade);

			// Add indexes for SalesOrderWeb
			modelBuilder.Entity<SalesOrderWeb>()
				.HasIndex(sow => sow.VoucherNumber)
				.IsUnique();

			modelBuilder.Entity<SalesOrderWeb>()
				.HasIndex(sow => sow.OrderDate);

			modelBuilder.Entity<SalesOrderWeb>()
				.HasIndex(sow => sow.BuyerName);

			modelBuilder.Entity<SalesOrderItemWeb>()
				.HasIndex(soiw => soiw.SalesOrderWebId);

			modelBuilder.Entity<SalesOrderItemWeb>()
				.HasIndex(soiw => soiw.ItemName);

			// Configure relationships
			modelBuilder.Entity<RollConfirmation>()
				.HasIndex(r => new { r.AllotId, r.MachineName, r.RollNo })
				.IsUnique();

			modelBuilder.Entity<Inspection>()
				.HasIndex(i => new { i.AllotId, i.MachineName, i.RollNo })
				.IsUnique();

			modelBuilder.Entity<ProductionAllotment>()
				.HasMany(pa => pa.MachineAllocations)
				.WithOne(ma => ma.ProductionAllotment)
				.HasForeignKey(ma => ma.ProductionAllotmentId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configure decimal precision for all decimal properties
			foreach (var property in modelBuilder.Model.GetEntityTypes()
				.SelectMany(t => t.GetProperties())
				.Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
			{
				property.SetPrecision(18);
				property.SetScale(3);
			}

			// Specific configuration for Efficiency property
			modelBuilder.Entity<ProductionAllotment>()
				.Property(p => p.Efficiency)
				.HasPrecision(5, 2);

			// Specific configuration for TotalProductionTime and EstimatedProductionTime
			modelBuilder.Entity<ProductionAllotment>()
				.Property(p => p.TotalProductionTime)
				.HasPrecision(18, 2);

			modelBuilder.Entity<MachineAllocation>()
				.Property(m => m.EstimatedProductionTime)
				.HasPrecision(18, 2);

			// Configure RollAssignment relationship
			modelBuilder.Entity<RollAssignment>()
				.HasOne(ra => ra.MachineAllocation)
				.WithMany(ma => ma.RollAssignments)
				.HasForeignKey(ra => ra.MachineAllocationId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configure GeneratedBarcode relationship
			modelBuilder.Entity<GeneratedBarcode>()
				.HasOne(gb => gb.RollAssignment)
				.WithMany(ra => ra.GeneratedBarcodes)
				.HasForeignKey(gb => gb.RollAssignmentId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configure StorageCapture indexes
			modelBuilder.Entity<StorageCapture>()
				.HasIndex(s => s.LotNo);

			modelBuilder.Entity<StorageCapture>()
				.HasIndex(s => s.FGRollNo);

			modelBuilder.Entity<StorageCapture>()
				.HasIndex(s => s.LocationCode);

			modelBuilder.Entity<StorageCapture>()
				.HasIndex(s => s.CustomerName);
				
			// Configure DispatchPlanning indexes
			modelBuilder.Entity<DispatchPlanning>()
				.HasIndex(dp => dp.LotNo);
				
			modelBuilder.Entity<DispatchPlanning>()
				.HasIndex(dp => dp.SalesOrderId);
				
			modelBuilder.Entity<DispatchPlanning>()
				.HasIndex(dp => dp.CustomerName);
				
			modelBuilder.Entity<DispatchPlanning>()
				.HasIndex(dp => dp.LoadingNo)
				.IsUnique();
				
			modelBuilder.Entity<DispatchPlanning>()
				.HasIndex(dp => dp.TransportId);
				
			modelBuilder.Entity<DispatchPlanning>()
				.HasIndex(dp => dp.CourierId);
				
			// Add indexes for new fields
			modelBuilder.Entity<DispatchPlanning>()
				.HasIndex(dp => dp.TransportName);
				
			modelBuilder.Entity<DispatchPlanning>()
				.HasIndex(dp => dp.ContactPerson);
				
			modelBuilder.Entity<DispatchedRoll>()
				.HasIndex(dr => dr.LotNo);
				
			modelBuilder.Entity<DispatchedRoll>()
				.HasIndex(dr => dr.FGRollNo);
				
			modelBuilder.Entity<DispatchedRoll>()
				.HasIndex(dr => dr.DispatchPlanningId);
				
			// Configure DispatchedRoll relationship
			modelBuilder.Entity<DispatchedRoll>()
				.HasOne(dr => dr.DispatchPlanning)
				.WithMany()
				.HasForeignKey(dr => dr.DispatchPlanningId)
				.OnDelete(DeleteBehavior.Cascade);
				
			// Configure DispatchPlanning relationships
			modelBuilder.Entity<DispatchPlanning>()
				.HasOne(dp => dp.Transport)
				.WithMany()
				.HasForeignKey(dp => dp.TransportId)
				.OnDelete(DeleteBehavior.SetNull);
				
			modelBuilder.Entity<DispatchPlanning>()
				.HasOne(dp => dp.Courier)
				.WithMany()
				.HasForeignKey(dp => dp.CourierId)
				.OnDelete(DeleteBehavior.SetNull);
		}

		public override int SaveChanges()
		{
			UpdateTimestampsAndUserFields();
			return base.SaveChanges();
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			UpdateTimestampsAndUserFields();
			return await base.SaveChangesAsync(cancellationToken);
		}

		private void UpdateTimestampsAndUserFields()
		{
			var entries = ChangeTracker.Entries<BaseEntity>()
				.Where(e => e.Entity is BaseEntity && (
					e.State == EntityState.Added ||
					e.State == EntityState.Modified));

			var currentUser = GetCurrentUser();

			foreach (var entry in entries)
			{
				if (entry.State == EntityState.Added)
				{
					((BaseEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
					((BaseEntity)entry.Entity).CreatedBy = currentUser;
				}

				if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
				{
					((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
					((BaseEntity)entry.Entity).UpdatedBy = currentUser;
				}
			}
		}

		private string GetCurrentUser()
		{
			if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
			{
				// Try to get the user's full name first
				var fullName = _httpContextAccessor.HttpContext.User.FindFirst("FullName")?.Value;
				if (!string.IsNullOrEmpty(fullName))
				{
					return fullName;
				}

				// Fallback to email
				var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
				if (!string.IsNullOrEmpty(email))
				{
					return email;
				}
			}

			// Default value when no user is authenticated
			return "System";
		}
	}
}