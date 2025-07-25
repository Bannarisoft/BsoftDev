using MediatR;

namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.DeleteNotificationLevelHierarchy
{
    public class DeleteNotificationLevelHierarchyCommand : IRequest<int> 
    {
        public int Id { get; set; }
    }
}