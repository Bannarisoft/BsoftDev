using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;

namespace BackgroundService.Domain.Entities.Notification
{
    public class NotificationEventRule : BaseEntity
    {
        public int NotificationLevelHierarchyId { get; set; }
        public int NotificationChannelId { get; set; }
        public int RecipientTypeId { get; set; }
        public int TemplateId { get; set; }        
        public MiscMaster RecipientType { get; set; }= new MiscMaster();
        public MiscMaster Channel { get; set; }= new MiscMaster();
        public NotificationTemplate NotificationTemplates { get; set; }= new NotificationTemplate();
        public NotificationLevelHierarchy NotificationLevelHierarchy { get; set; }= new NotificationLevelHierarchy();
        public ICollection<NotificationEventLog> NotificationEventLog { get; set; }= new List<NotificationEventLog>();
        
    }
}