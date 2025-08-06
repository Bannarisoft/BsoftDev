using MediatR;

namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.UpdateNotificationLevelHierarchy
{
    public class UpdateNotificationLevelHierarchyCommand : IRequest<int>
    {
        public int Id { get; set; }
       public int NotificationConfigId { get; set; }
        public int TargetTypeId { get; set; }
        public int TargetId { get; set; }        
        public int ApprovalModeId { get; set; }        
        public string? Description { get; set; }
        public byte IsActive { get; set; }
    }
}