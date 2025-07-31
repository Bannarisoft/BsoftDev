using System;
using System.Collections.Generic;

namespace Contracts.Events.Notifications.WorkOrder
{
    public class ResolveNotificationChannelsResponse
    {
        public Guid CorrelationId { get; set; }   
        public List<string> Channels { get; set; } = [];
    }
}