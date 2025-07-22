using System;
using System.Collections.Generic;
using MassTransit;

namespace Contracts.Events.Notifications.WorkOrder.Email
{
    public class SendEmailNotificationInternalCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public int UnitId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
        public int EventRuleId { get; set; }
        public int ChannelId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string param1 { get; set; } = string.Empty;
        public string param2 { get; set; } = string.Empty;
        public DateTimeOffset param3 { get; set; }
        public int RetryCount { get; set; } = 0; 
    }
}
