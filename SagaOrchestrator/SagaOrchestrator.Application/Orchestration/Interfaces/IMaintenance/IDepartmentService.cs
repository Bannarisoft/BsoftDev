using Contracts.Dtos.Maintenance;

namespace SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance
{
    public interface IDepartmentService
    {
        Task<DepartmentDto> GetDepartmentByIdAsync(int departmentId, string token);
    }
}