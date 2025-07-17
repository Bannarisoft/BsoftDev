using System;
using MassTransit;

namespace Contracts.Events.Notifications.WorkOrder.Sms
{
    public class SendSmsNotificationCompleted : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
