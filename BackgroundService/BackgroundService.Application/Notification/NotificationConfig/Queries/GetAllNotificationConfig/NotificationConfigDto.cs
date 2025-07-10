namespace BackgroundService.Application.Notification.NotificationConfig.Queries.GetAllNotificationConfig
{
    public class NotificationConfigDto
    {   
        public int Id { get; set; }
        public string? ModuleName { get; set; }        
        public int NotificationEventTypeId { get; set; }        
    }
}