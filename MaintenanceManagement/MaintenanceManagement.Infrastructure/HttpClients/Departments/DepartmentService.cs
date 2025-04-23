using System.Net.Http.Json;
using Contracts.Dtos.Maintenance;
using Core.Application.Common.HttpResponse;
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

    var request = new HttpRequestMessage(HttpMethod.Get, "api/Department/by-name");
    var response = await client.SendAsync(request);

    if (response.IsSuccessStatusCode)
    {
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<List<DepartmentDto>>>();
        return result?.Data ?? new List<DepartmentDto>();
    }

    // Optional: handle failure
    return new List<DepartmentDto>();
        }
    }
}