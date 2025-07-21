using System;
using System.Collections.Generic;
using MassTransit;

namespace Contracts.Events.Notifications.WorkOrder.Sms
{
    public class SendSmsNotificationInternalCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public int UnitId { get; set; }
        public int EventTypeId { get; set; }
        public string ModuleName { get; set; }
        public List<string> MobileNumbers { get; set; } = new();
        public int ChannelId { get; set; }
        public int EventRuleId { get; set; }   
        public string CreatedByName { get; set; } = string.Empty;        
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }      
    }
}
