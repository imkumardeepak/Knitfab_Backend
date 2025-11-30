using AvyyanBackend.WebSockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AvyyanBackend.Services
{
    public interface ISignalRNotificationService
    {
        Task SendNotificationAsync(string message);
        Task SendNotificationToUserAsync(string userId, string message);
        Task SendChatMessageAsync(string message);
    }

    public class SignalRNotificationService : ISignalRNotificationService
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly ILogger<SignalRNotificationService> _logger;

        public SignalRNotificationService(
            IHubContext<NotificationHub> notificationHubContext,
            IHubContext<ChatHub> chatHubContext,
            ILogger<SignalRNotificationService> logger)
        {
            _notificationHubContext = notificationHubContext;
            _chatHubContext = chatHubContext;
            _logger = logger;
        }

        public async Task SendNotificationAsync(string message)
        {
            try
            {
                await _notificationHubContext.Clients.All.SendAsync("ReceiveNotification", message);
                _logger.LogInformation($"Notification sent to all clients: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to all clients");
            }
        }

        public async Task SendNotificationToUserAsync(string userId, string message)
        {
            try
            {
                await _notificationHubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
                _logger.LogInformation($"Notification sent to user {userId}: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to user {userId}");
            }
        }

        public async Task SendChatMessageAsync(string message)
        {
            try
            {
                await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", message);
                _logger.LogInformation($"Chat message sent to all clients: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending chat message to all clients");
            }
        }
    }
}