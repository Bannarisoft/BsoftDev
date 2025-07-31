using System;
using MassTransit;

namespace Contracts.Events.Notifications.WorkOrder
{
    public class NotificationSagaRollbackTriggered : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
        public string Reason { get; set; }
    }
}
