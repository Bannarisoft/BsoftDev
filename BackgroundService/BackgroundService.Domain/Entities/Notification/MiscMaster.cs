using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;

namespace BackgroundService.Domain.Entities.Notification
{
    public class MiscMaster : BaseEntity
    {
        public int MiscTypeId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public MiscTypeMaster MiscType { get; set; }
        public ICollection<NotificationLevelHierarchy> TargetType { get; set; } = new List<NotificationLevelHierarchy>();
        public ICollection<NotificationLevelHierarchy> ApprovalMode { get; set; } = new List<NotificationLevelHierarchy>();
        public ICollection<NotificationEventRule> NotificationType { get; set; } = new List<NotificationEventRule>();
        public ICollection<NotificationEventRule> NotificationStatus { get; set; } = new List<NotificationEventRule>();
        public ICollection<NotificationEventRule> RecipientType { get; set; } = new List<NotificationEventRule>();
        public ICollection<NotificationConfig> NotificationEventType { get; set; } = new List<NotificationConfig>();
        public ICollection<NotificationEventLog> Channel { get; set; } = new List<NotificationEventLog>(); 
        public ICollection<NotificationTemplate> NotificationTemplates { get; set; } = new List<NotificationTemplate>();
    }
}