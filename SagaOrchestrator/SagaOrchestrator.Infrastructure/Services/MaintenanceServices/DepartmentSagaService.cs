using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Maintenance;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance;

namespace SagaOrchestrator.Infrastructure.Services.MaintenanceServices
{
    public class DepartmentSagaService
    {
         private readonly IDepartmentService _departmentService;
        private readonly IPublishEndpoint _publishEndpoint;
        public DepartmentSagaService(IDepartmentService departmentService, IPublishEndpoint publishEndpoint)
        {   
            _departmentService = departmentService;
            _publishEndpoint = publishEndpoint;
        }
        public async Task TriggerDepartmentCreation(int departmentId)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(departmentId);

            if (department != null)
            {
                var departmentCreatedEvent = new DepartmentCreatedEvent
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.DepartmentName
                };

                await _publishEndpoint.Publish(departmentCreatedEvent);
            }
        }
    }


}