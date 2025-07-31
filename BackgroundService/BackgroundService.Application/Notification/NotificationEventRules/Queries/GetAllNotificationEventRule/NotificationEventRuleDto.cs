using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Notification.NotificationEventRules.Queries.GetAllNotificationEventRule
{
    public class NotificationEventRuleDto
    {
        public int Id { get; set; }
        public int NotificationChannelId { get; set; }
        public int TemplateId { get; set; }
        public int NotificationLevelHierarchyId { get; set; }
        public int RecipientTypeId { get; set; } 
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public int ModifiedBy { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string ModifiedByName { get; set; }
    }
}