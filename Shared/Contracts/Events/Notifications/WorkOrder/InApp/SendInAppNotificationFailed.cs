using System;
using MassTransit;

namespace Contracts.Events.Notifications.WorkOrder.InApp
{
    public class SendInAppNotificationFailed : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
