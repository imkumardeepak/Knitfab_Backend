using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvyyanBackend.Services;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly ISignalRNotificationService _signalRNotificationService;

        public WebSocketController(ISignalRNotificationService signalRNotificationService)
        {
            _signalRNotificationService = signalRNotificationService;
        }

        [HttpPost("/api/notification")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            await _signalRNotificationService.SendNotificationAsync(request.Message);
            return Ok("Notification sent.");
        }

        [HttpPost("/api/notification/user/{userId}")]
        public async Task<IActionResult> SendNotificationToUser(string userId, [FromBody] NotificationRequest request)
        {
            await _signalRNotificationService.SendNotificationToUserAsync(userId, request.Message);
            return Ok($"Notification sent to user {userId}.");
        }
    }

    public class NotificationRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}