using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.WebSocket
{
    /// <summary>
    /// DTO for WebSocket connection request
    /// </summary>
    public class WebSocketConnectionRequestDto
    {
        [Required]
        public string EmployeeId { get; set; } = string.Empty;
        public string? ConnectionId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO for chat message
    /// </summary>
    public class ChatMessageDto
    {
        public string Id { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string? GroupId { get; set; } = null; // For group chat
        public string Message { get; set; } = string.Empty;
        public string MessageType { get; set; } = "text"; // text, image, file, etc.
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string? ReplyToMessageId { get; set; } = null; // For replies
        public Dictionary<string, object>? Metadata { get; set; } = null; // For additional data
    }

    /// <summary>
    /// DTO for group chat creation
    /// </summary>
    public class CreateGroupChatDto
    {
        [Required]
        public string GroupName { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public string CreatorId { get; set; } = string.Empty;
        [Required]
        public List<string> MemberIds { get; set; } = new List<string>();
        public string GroupType { get; set; } = "private"; // private, public, broadcast
    }

    /// <summary>
    /// DTO for group chat information
    /// </summary>
    public class GroupChatDto
    {
        public string Id { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CreatorId { get; set; } = string.Empty;
        public List<string> MemberIds { get; set; } = new List<string>();
        public string GroupType { get; set; } = "private";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for notification
    /// </summary>
    public class NotificationDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // info, warning, error, success
        public string Category { get; set; } = "general"; // general, chat, system, machine
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public Dictionary<string, object>? Data { get; set; } = null; // Additional payload
        public string? ActionUrl { get; set; } = null; // URL for action button
        public DateTime? ExpiresAt { get; set; } = null; // For time-sensitive notifications
    }

    /// <summary>
    /// DTO for WebSocket response
    /// </summary>
    public class WebSocketResponseDto
    {
        public string Type { get; set; } = string.Empty;
        public object Data { get; set; } = new object();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; } = true;
        public string? Error { get; set; } = null;
        public string? MessageId { get; set; } = null; // For tracking specific messages
    }

    /// <summary>
    /// DTO for typing indicator
    /// </summary>
    public class TypingIndicatorDto
    {
        public string UserId { get; set; } = string.Empty;
        public string? ChatId { get; set; } = null; // For direct chat
        public string? GroupId { get; set; } = null; // For group chat
        public bool IsTyping { get; set; } = false;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO for message read receipt
    /// </summary>
    public class ReadReceiptDto
    {
        public string MessageId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO for user presence status
    /// </summary>
    public class UserPresenceDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = "online"; // online, offline, away, busy
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public string? CustomStatus { get; set; } = null;
    }

    /// <summary>
    /// DTO for heartbeat/ping messages
    /// </summary>
    public class HeartbeatDto
    {
        public string Type { get; set; } = "ping"; // ping, pong
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; } = null;
    }

    /// <summary>
    /// DTO for WebSocket message wrapper
    /// </summary>
    public class WebSocketMessageDto
    {
        public string Type { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public object? Payload { get; set; } = null;
        public string? RequestId { get; set; } = null; // For request-response tracking
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO for bulk notifications
    /// </summary>
    public class BulkNotificationDto
    {
        public List<string> UserIds { get; set; } = new List<string>();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "info";
        public string Category { get; set; } = "general";
        public Dictionary<string, object>? Data { get; set; } = null;
        public string? ActionUrl { get; set; } = null;
        public DateTime? ExpiresAt { get; set; } = null;
    }
}