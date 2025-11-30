using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AvyyanBackend.WebSockets
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(string message)
        {
            try
            {
                var chatMessage = JsonSerializer.Deserialize<ChatMessage>(message) ?? new ChatMessage();
                chatMessage.SenderId = Context.UserIdentifier ?? "Unknown";
                chatMessage.Timestamp = DateTime.UtcNow;

                _logger.LogInformation($"Received message from {chatMessage.SenderId}: {chatMessage.Message}");

                // Private message
                if (!string.IsNullOrEmpty(chatMessage.ReceiverId))
                {
                    await Clients.User(chatMessage.ReceiverId).SendAsync("ReceiveMessage", JsonSerializer.Serialize(chatMessage));
                    _logger.LogInformation($"Sent private message to {chatMessage.ReceiverId}");
                }
                // Group message (broadcast)
                else
                {
                    await Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(chatMessage));
                    _logger.LogInformation("Broadcasted message to all users");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Invalid message format: {ex.Message}");
                await Clients.Caller.SendAsync("ErrorMessage", "Invalid message format");
            }
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"User {Context.UserIdentifier} joined group {groupName}");
            await Clients.Group(groupName).SendAsync("UserJoined", Context.UserIdentifier, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"User {Context.UserIdentifier} left group {groupName}");
            await Clients.Group(groupName).SendAsync("UserLeft", Context.UserIdentifier, groupName);
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            var chatMessage = new ChatMessage
            {
                SenderId = Context.UserIdentifier ?? "Unknown",
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Group(groupName).SendAsync("ReceiveMessage", JsonSerializer.Serialize(chatMessage));
            _logger.LogInformation($"Sent message to group {groupName}");
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Chat client connected: {Context.ConnectionId}, User: {Context.UserIdentifier}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Chat client disconnected: {Context.ConnectionId}, User: {Context.UserIdentifier}");
            await base.OnDisconnectedAsync(exception);
        }
    }

    public class ChatMessage
    {
        public string? SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? Message { get; set; }
        public string? ReceiverId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}