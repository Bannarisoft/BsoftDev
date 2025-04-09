using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events.Maintenance;
using Contracts.Models.Maintenance;
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
        public async Task<DepartmentDto> TriggerDepartmentCreation(int departmentId,string token)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(departmentId, token);

            if (department != null)
            {
                var departmentCreatedEvent = new DepartmentCreatedEvent
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.DepartmentName,
                    ShortName = department.ShortName
                };

                await _publishEndpoint.Publish(departmentCreatedEvent);
                return department;
            }
            return null;
        }
    }


}