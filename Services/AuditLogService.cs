using System.Security.Claims;
using System.Text.Json;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.AuditLog;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditLogService> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public AuditLogService(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditLogService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // ── Write ─────────────────────────────────────────────────────────────

        public async Task LogAsync(CreateAuditLogDto dto)
        {
            try
            {
                // Enrich from HTTP context if not explicitly provided
                EnrichFromHttpContext(dto);

                var entry = new AuditLog
                {
                    UserId = dto.UserId,
                    UserName = dto.UserName,
                    UserRole = dto.UserRole,
                    Action = dto.Action,
                    Module = dto.Module,
                    EntityId = dto.EntityId,
                    EntityName = dto.EntityName,
                    OldValues = dto.OldValues != null ? JsonSerializer.Serialize(dto.OldValues, _jsonOptions) : null,
                    NewValues = dto.NewValues != null ? JsonSerializer.Serialize(dto.NewValues, _jsonOptions) : null,
                    ChangeSummary = dto.ChangeSummary,
                    IpAddress = dto.IpAddress,
                    IsSystemAction = dto.IsSystemAction,
                    Timestamp = DateTime.UtcNow
                };

                // Use a fresh DbContext to avoid SaveChanges interceptor re-entry issues
                await using var db = await _dbContextFactory.CreateDbContextAsync();
                db.AuditLogs.Add(entry);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Audit logging must NEVER crash the main request pipeline
                _logger.LogError(ex, "Failed to write audit log. Action={Action}, Module={Module}", dto.Action, dto.Module);
            }
        }

        public async Task LogAsync(
            string action,
            string module,
            string? changeSummary = null,
            int? entityId = null,
            string? entityName = null,
            object? oldValues = null,
            object? newValues = null,
            bool isSystemAction = false)
        {
            await LogAsync(new CreateAuditLogDto
            {
                Action = action,
                Module = module,
                ChangeSummary = changeSummary,
                EntityId = entityId,
                EntityName = entityName,
                OldValues = oldValues,
                NewValues = newValues,
                IsSystemAction = isSystemAction
            });
        }

        // ── Query ─────────────────────────────────────────────────────────────

        public async Task<AuditLogPagedResultDto> GetLogsAsync(AuditLogQueryDto query)
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();

            var q = db.AuditLogs.AsNoTracking().AsQueryable();

            if (query.From.HasValue)
                q = q.Where(l => l.Timestamp >= query.From.Value.ToUniversalTime());

            if (query.To.HasValue)
                q = q.Where(l => l.Timestamp <= query.To.Value.ToUniversalTime());

            if (!string.IsNullOrWhiteSpace(query.Module))
                q = q.Where(l => l.Module == query.Module);

            if (!string.IsNullOrWhiteSpace(query.Action))
                q = q.Where(l => l.Action == query.Action);

            if (query.UserId.HasValue)
                q = q.Where(l => l.UserId == query.UserId.Value);

            if (!string.IsNullOrWhiteSpace(query.EntityName))
                q = q.Where(l => l.EntityName != null && EF.Functions.ILike(l.EntityName, $"%{query.EntityName}%"));

            var totalCount = await q.CountAsync();

            var items = await q
                .OrderByDescending(l => l.Timestamp)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(l => new AuditLogDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    UserName = l.UserName,
                    UserRole = l.UserRole,
                    Action = l.Action,
                    Module = l.Module,
                    EntityId = l.EntityId,
                    EntityName = l.EntityName,
                    OldValues = l.OldValues,
                    NewValues = l.NewValues,
                    ChangeSummary = l.ChangeSummary,
                    IpAddress = l.IpAddress,
                    IsSystemAction = l.IsSystemAction,
                    Timestamp = l.Timestamp
                })
                .ToListAsync();

            return new AuditLogPagedResultDto
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<List<string>> GetModulesAsync()
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            return await db.AuditLogs
                .AsNoTracking()
                .Select(l => l.Module)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }

        public async Task<List<string>> GetActionsAsync()
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            return await db.AuditLogs
                .AsNoTracking()
                .Select(l => l.Action)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void EnrichFromHttpContext(CreateAuditLogDto dto)
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null) return;

            // IP
            if (string.IsNullOrEmpty(dto.IpAddress))
            {
                dto.IpAddress = ctx.Connection.RemoteIpAddress?.ToString();
            }

            // User identity
            if (ctx.User?.Identity?.IsAuthenticated == true)
            {
                if (dto.UserId == null)
                {
                    var userIdStr = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdStr, out var uid))
                        dto.UserId = uid;
                }

                if (dto.UserName == "System")
                {
                    var fullName = ctx.User.FindFirst("FullName")?.Value;
                    dto.UserName = !string.IsNullOrEmpty(fullName)
                        ? fullName
                        : ctx.User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
                }

                if (string.IsNullOrEmpty(dto.UserRole))
                {
                    dto.UserRole = ctx.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                }
            }
        }
    }
}
