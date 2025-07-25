using BackgroundService.Application.DTO;
using BackgroundService.Application.Hubs;
using BackgroundService.Application.Interfaces;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Domain.Entities.Notification;
using Contracts.Events.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BackgroundService.Infrastructure.Services.Notification
{
    public class InAppNotifier : IInAppNotifier
    {
        private readonly ILogger<InAppNotifier> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;        
        private readonly INotificationLogger _loggerNotification;

        public InAppNotifier(ILogger<InAppNotifier> logger, IHubContext<NotificationHub> hubContext, INotificationLogger loggerNotification)
        {
            _logger = logger;
            _hubContext = hubContext;
            _loggerNotification = loggerNotification;            
        }

        public async Task<bool> SendInAppNotificationAsync(List<int> userIds, string message, string title, NotificationContext context)
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
                    var log = new NotificationEventLog
                    {
                        NotificationLevelRuleId = context.EventRuleId,
                        NotificationStatusId = (int)NotificationEnum.NotificationStatus.Success,
                        ReadStatusId = (int)NotificationEnum.NotificationReadStatus.Unread,
                        SendTo = id.ToString(),
                        ActionStatus = "Sent",
                        ChannelId =(int)NotificationEnum.NotificationChannel.InApp,
                        MessageText = message,
                        Timestamp = DateTime.UtcNow,
                        CreatedBy = context.CreatedById,
                        CreatedDate = DateTime.UtcNow,
                        CreatedByName = context.CreatedByName,
                        CreatedIP = context.CreatedIp
                    };
                    var savedLogId = await _loggerNotification.LogAsync(log);
                    //await _hubContext.Clients.User(id.ToString()).SendAsync("ReceiveNotification", message);
                    await _hubContext.Clients.User(id.ToString()).SendAsync("ReceiveNotification", new
                    {
                        LogId = savedLogId,
                        Message = message,
                        Title = title,
                        Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        Type = "info" 
                    });
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
