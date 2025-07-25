using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;

namespace BackgroundService.Domain.Entities.Notification
{
    public class NotificationEventLog : BaseEntity
    {
        public int NotificationLevelRuleId { get; set; }
        public int ChannelId { get; set; }
        public int NotificationStatusId { get; set; }
        public string? MessageText { get; set; }
        public string? ActionStatus { get; set; }
        public int ReadStatusId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string? SendTo { get; set; }
        public NotificationEventRule NotificationEventRules { get; set; } = new NotificationEventRule();
        public MiscMaster Channel { get; set; } = new MiscMaster();
        public MiscMaster NotificationStatus { get; set; } = new MiscMaster();
        public MiscMaster ReadStatus { get; set; }= new MiscMaster();
    }
}