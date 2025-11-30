using AvyyanBackend.Data;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Services
{
    public class DataSeedService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataSeedService> _logger;

        public DataSeedService(ApplicationDbContext context, ILogger<DataSeedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedRolesAsync();
                await SeedDefaultUserAsync();
                await SeedPermissionsAsync();

                _logger.LogInformation("Data seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during data seeding");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            if (await _context.RoleMasters.AnyAsync())
            {
                _logger.LogInformation("Roles already seeded.");
                return;
            }

            var roles = new[]
            {
                new RoleMaster { RoleName = "Admin", Description = "System Administrator with full access", IsActive = true },
            };

            _context.RoleMasters.AddRange(roles);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded default roles.");
        }

        private async Task SeedDefaultUserAsync()
        {
            if (!await _context.Users.AnyAsync(u => u.Email == "admin@gmail.com"))
            {
                var adminUser = new User
                {
                    FirstName = "Avyaan",
                    LastName = "Infotech",
                    Email = "admin@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123"), // Change this in production!
                    IsActive = true,
                    RoleName = "Admin",
                    PhoneNumber = "1234567890",
                    CreatedBy = "System",
					UpdatedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
				};

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created default admin user: admin@avyyan.com / admin123");
            }
        }

        private async Task SeedPermissionsAsync()
        {
            if (await _context.PageAccesses.AnyAsync())
            {
                _logger.LogInformation("Page access permissions already seeded.");
                return;
            }

            var adminRole = await _context.RoleMasters.SingleOrDefaultAsync(r => r.RoleName == "Admin");

            if (adminRole == null)
            {
                _logger.LogError("One or more default roles not found. Skipping permission seeding.");
                return;
            }

            var pageAccesses = new List<PageAccess>();

            var allPages = new[] { "Dashboard", "Machines", "Machine Details", "Chat", "Notifications", "Users", "Reports", "Machine Reports", "Settings", "Profile", "Production Allotment", "Tape Color", "Shift Master" };

            // Admin: Full access
            foreach (var page in allPages)
            {
                pageAccesses.Add(new PageAccess { RoleId = adminRole.Id, PageName = page, IsView = true, IsAdd = true, IsEdit = true, IsDelete = true });
            }

            _context.PageAccesses.AddRange(pageAccesses);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded page access permissions.");
        }
    }
}