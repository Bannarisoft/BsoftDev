using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;

namespace BackgroundService.Domain.Entities.Notification
{
    public class NotificationEventLog : BaseEntity
    {
        public int NotificationLevelHierarchyId { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public int ChannelId { get; set; }
        public string MessageText { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public NotificationLevelHierarchy NotificationLevelHierarchy { get; set; }
        public MiscMaster Channel { get; set; }
    }
}