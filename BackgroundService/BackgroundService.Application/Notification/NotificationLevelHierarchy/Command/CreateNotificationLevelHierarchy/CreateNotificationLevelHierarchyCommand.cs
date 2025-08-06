using MediatR;

namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.CreateNotificationLevelHierarchy
{
    public class CreateNotificationLevelHierarchyCommand : IRequest<int>
    {        
        public int NotificationConfigId { get; set; }
        public int TargetTypeId { get; set; }
        public int TargetId { get; set; }        
        public int ApprovalModeId { get; set; }        
        public string? Description { get; set; }
    }
}