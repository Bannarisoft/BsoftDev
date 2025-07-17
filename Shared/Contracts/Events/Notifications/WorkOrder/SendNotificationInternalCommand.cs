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
        public string Channel { get; set; } = string.Empty; 
    }
}
