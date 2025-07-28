using System;
using MassTransit;

namespace Contracts.Events.Notifications.WorkOrder.InApp
{
    public class SendInAppNotificationCompleted : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
