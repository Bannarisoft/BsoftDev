using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts.Models.Maintenance;
using SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance;

namespace SagaOrchestrator.Application.Orchestration.Services.MaintenanceServices
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HttpClient _httpClient;
        public DepartmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<DepartmentDto> GetDepartmentByIdAsync(int departmentId)
        {
            var response = await _httpClient.GetAsync($"/api/Departments/{departmentId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DepartmentDto>(content);
        }
    }
}