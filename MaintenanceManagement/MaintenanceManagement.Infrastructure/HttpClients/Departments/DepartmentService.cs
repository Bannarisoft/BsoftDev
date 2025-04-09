using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Contracts.Models.Maintenance;
using Core.Application.Common.Interfaces.External.IDepartment;

namespace MaintenanceManagement.Infrastructure.HttpClients.Departments
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HttpClient _httpClient;
        public DepartmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<DepartmentDto>> GetAllDepartmentAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<DepartmentDto>>("api/Department");
            return response ?? new List<DepartmentDto>();
        }
    }
}