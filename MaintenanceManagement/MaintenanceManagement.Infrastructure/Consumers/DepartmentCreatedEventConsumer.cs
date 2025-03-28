using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Maintenance;
using MassTransit;

namespace MaintenanceManagement.Infrastructure.Consumers
{
    public class DepartmentCreatedEventConsumer : IConsumer<DepartmentCreatedEvent>
    {
        public async Task Consume(ConsumeContext<DepartmentCreatedEvent> context)
        {
            var department = context.Message;
            Console.WriteLine($"Department Created: {department.DepartmentName} with ID: {department.DepartmentId}");
        }
    }
}