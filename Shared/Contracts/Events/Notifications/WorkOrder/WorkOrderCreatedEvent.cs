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

        
        public string param1 { get; set; }   
        public string param2 { get; set; }   
        public DateTimeOffset param3 { get; set; }  
    }
}
