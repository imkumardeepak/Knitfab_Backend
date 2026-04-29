namespace AvyyanBackend.DTOs.AuditLog
{
    // ── Write ────────────────────────────────────────────────────────────────

    public class CreateAuditLogDto
    {
        public int? UserId { get; set; }
        public string UserName { get; set; } = "System";
        public string UserRole { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string? EntityName { get; set; }
        public object? OldValues { get; set; }
        public object? NewValues { get; set; }
        public string? ChangeSummary { get; set; }
        public string? IpAddress { get; set; }
        public bool IsSystemAction { get; set; } = false;
    }

    // ── Query ────────────────────────────────────────────────────────────────

    public class AuditLogQueryDto
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Module { get; set; }
        public string? Action { get; set; }
        public int? UserId { get; set; }
        public string? EntityName { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    // ── Read ─────────────────────────────────────────────────────────────────

    public class AuditLogDto
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string? EntityName { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? ChangeSummary { get; set; }
        public string? IpAddress { get; set; }
        public bool IsSystemAction { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AuditLogPagedResultDto
    {
        public List<AuditLogDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
