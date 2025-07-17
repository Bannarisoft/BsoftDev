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
        public List<string> PlaceholderData { get; set; } 
    }
}
