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
        public int NotificationTypeId { get; set; }
        public int NotificationStatusId { get; set; }
        public int NotificationConfigId { get; set; }
        public int NotificationGroupId { get; set; }
        public int RecipientTypeId { get; set; }
        public MiscMaster NotificationType { get; set; }
        public MiscMaster NotificationStatus { get; set; }
        public NotificationConfig NotificationConfig { get; set; }
        public NotificationGroup NotificationGroup { get; set; }
        public MiscMaster RecipientType { get; set; }
    }
}