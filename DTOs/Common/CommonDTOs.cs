using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.Common
{
    /// <summary>
    /// Generic response wrapper for API responses
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Generic pagination wrapper for list responses
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    public class PaginatedResponseDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
    }

    /// <summary>
    /// Generic message response DTO
    /// </summary>
    public class MessageResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; } = true;
    }

    /// <summary>
    /// Generic error response DTO
    /// </summary>
    public class ErrorResponseDto
    {
        public string Error { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int StatusCode { get; set; }
    }

    /// <summary>
    /// Base audit DTO for entities with audit fields
    /// </summary>
    public abstract class BaseAuditDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}