using AvyyanBackend.DTOs.AuditLog;

namespace AvyyanBackend.Interfaces
{
    public interface IAuditLogService
    {
        /// <summary>
        /// Write a single audit log entry. Fire-and-forget safe — swallows exceptions to avoid
        /// disrupting the main request pipeline.
        /// </summary>
        Task LogAsync(CreateAuditLogDto dto);

        /// <summary>
        /// Convenience overload for the most common case.
        /// </summary>
        Task LogAsync(
            string action,
            string module,
            string? changeSummary = null,
            int? entityId = null,
            string? entityName = null,
            object? oldValues = null,
            object? newValues = null,
            bool isSystemAction = false
        );

        /// <summary>
        /// Paginated query endpoint used by the Audit Log page.
        /// </summary>
        Task<AuditLogPagedResultDto> GetLogsAsync(AuditLogQueryDto query);

        /// <summary>
        /// Get all distinct module names (for filter dropdowns).
        /// </summary>
        Task<List<string>> GetModulesAsync();

        /// <summary>
        /// Get all distinct action types (for filter dropdowns).
        /// </summary>
        Task<List<string>> GetActionsAsync();
    }
}
