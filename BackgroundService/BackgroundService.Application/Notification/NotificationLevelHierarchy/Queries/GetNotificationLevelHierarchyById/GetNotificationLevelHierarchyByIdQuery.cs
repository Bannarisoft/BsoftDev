using BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetAllNotificationLevelHierarchy;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetNotificationLevelHierarchyById
{
    public class GetNotificationLevelHierarchyByIdQuery : IRequest<NotificationLevelHierarchyDto>
    {
       public int Id { get; set; }
    }
}