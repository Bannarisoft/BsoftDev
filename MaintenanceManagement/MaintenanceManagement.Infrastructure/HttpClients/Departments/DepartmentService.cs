using System.Net.Http.Json;
using Contracts.Models.Maintenance;
using Core.Application.Common.Interfaces.External.IDepartment;

namespace MaintenanceManagement.Infrastructure.HttpClients.Departments
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DepartmentService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<DepartmentDto>> GetAllDepartmentAsync()
        {
            var client = _httpClientFactory.CreateClient("DepartmentClient");
            var response = await client.GetFromJsonAsync<List<DepartmentDto>>("api/Department");
            return response ?? new List<DepartmentDto>();
        }
    }
}