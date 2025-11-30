using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AvyyanBackend.WebSockets
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task SendNotification(string message)
        {
            _logger.LogInformation($"Sending notification: {message}");
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task SendNotificationToUser(string userId, string message)
        {
            _logger.LogInformation($"Sending notification to user {userId}: {message}");
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}