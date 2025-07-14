using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;

namespace BackgroundService.Domain.Entities.Notification
{
    public class NotificationEventRule : BaseEntity
    {
        public int NotificationGroupMemberId { get; set; }        
        public int NotificationStatusId { get; set; }        
        public int NotificationGroupId { get; set; }
        public int RecipientTypeId { get; set; }
        public int TemplateId { get; set; }        
        public MiscMaster NotificationStatus { get; set; }        
        public NotificationGroup NotificationGroup { get; set; }
        public MiscMaster RecipientType { get; set; }
        public NotificationTemplate NotificationTemplates { get; set; }
        
    }
}