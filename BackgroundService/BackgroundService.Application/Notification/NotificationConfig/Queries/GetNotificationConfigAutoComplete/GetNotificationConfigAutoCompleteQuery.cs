using MediatR;

namespace BackgroundService.Application.Notification.NotificationConfig.Queries.GetNotificationConfigAutoComplete
{
    public class GetNotificationConfigAutoCompleteQuery : IRequest<List<GetNotificationConfigAutoCompleteDto>>    
    {
        public string? SearchPattern { get; set; }       
    }
}