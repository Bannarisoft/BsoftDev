namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetAllNotificationLevelHierarchy
{
    public class NotificationLevelHierarchyDto
    {
        public int Id { get; set; }
       public int NotificationConfigId { get; set; }
        public int TargetTypeId { get; set; }
        public int TargetId { get; set; }
        public int UnitId { get; set; }
        public int ApprovalModeId { get; set; }        
        public string? Description { get; set; }
        public string? ModuleName { get; set; }
        public string? NotificationEventType { get; set; }
        public string? TargetType { get; set; }
        public string? TargetName { get; set; }
        public string? ApprovalMode { get; set; }        
        public string? DepartmentName { get; set; }
        public int IsActive { get; set; }
        public int IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }

    }
}