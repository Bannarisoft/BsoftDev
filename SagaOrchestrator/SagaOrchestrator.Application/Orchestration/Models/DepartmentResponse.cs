using Contracts.Dtos.Maintenance;

namespace SagaOrchestrator.Application.Models
{
    public class DepartmentResponse
    {
         public int StatusCode { get; set; }
        public DepartmentDto? Data { get; set; }
    }
}