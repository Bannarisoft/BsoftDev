using System;
using MassTransit;

namespace Contracts.Events.Notifications.WorkOrder
{
    public record SendNotificationInternalCommand : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public int UnitId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int EventTypeId { get; set; }
        public string CreatedByName { get; set; }
        public int EventRuleId { get; set; }
        public int ChannelId { get; set; }

        
        public string param1 { get; set; }   
        public string param2 { get; set; }   
        public DateTimeOffset param3 { get; set; }  
    }
}
