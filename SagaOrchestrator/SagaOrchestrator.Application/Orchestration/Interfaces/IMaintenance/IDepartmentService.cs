using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.Maintenance;

namespace SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance
{
    public interface IDepartmentService
    {
        Task<DepartmentDto> GetDepartmentByIdAsync(int departmentId);
    }
}