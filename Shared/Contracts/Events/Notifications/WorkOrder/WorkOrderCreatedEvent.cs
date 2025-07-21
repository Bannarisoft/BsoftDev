// Contracts/Events/Notifications/WorkOrder/WorkOrderCreatedEvent.cs
using System;
using System.Collections.Generic;
using MassTransit;

namespace Contracts.Events.Notifications.WorkOrder
{
    public class WorkOrderCreatedEvent : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }                
        public string CreatedByName { get; set; }
        public int UnitId { get; set; }
        public int EventTypeId { get; set; }
        public string ModuleName { get; set; }

        
        public string Code { get; set; }   
        public string Name { get; set; }   
        public DateTimeOffset Date { get; set; }  
    }
}
