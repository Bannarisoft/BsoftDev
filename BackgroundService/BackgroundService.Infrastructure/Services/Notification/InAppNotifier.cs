using BackgroundService.Application.Hubs;
using BackgroundService.Application.Interfaces;
using BackgroundService.Application.Interfaces.Notification;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BackgroundService.Infrastructure.Services.Notification
{
    public class InAppNotifier : IInAppNotifier
    {
        private readonly ILogger<InAppNotifier> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public InAppNotifier(ILogger<InAppNotifier> logger, IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task<bool> SendInAppNotificationAsync(List<int> userIds, string message, string title)
        {
            try
            {
                if (userIds == null || !userIds.Any())
                {
                    _logger.LogWarning("⚠️ No users provided for in-app notification.");
                    return false;
                }
                foreach (var id in userIds)
                {
                    await _hubContext.Clients.User(id.ToString()).SendAsync("ReceiveNotification", message);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send in-app to users");
                return false;
            }
        }
    }
}
