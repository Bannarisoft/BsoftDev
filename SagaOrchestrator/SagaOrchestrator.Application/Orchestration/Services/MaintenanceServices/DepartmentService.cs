using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts.Models.Maintenance;
using Polly;
using Polly.Retry;
using SagaOrchestrator.Application.Models;
using SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance;
using Serilog;

namespace SagaOrchestrator.Application.Orchestration.Services.MaintenanceServices
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public DepartmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(3);
        }
        public async Task<DepartmentDto> GetDepartmentByIdAsync(int departmentId, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Department/{departmentId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(request));
                var content = await response.Content.ReadAsStringAsync();

                Log.Information("Department API raw response: {Content}", content); // Optional for debugging

                var result = JsonSerializer.Deserialize<DepartmentResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result.Data == null)
                {
                    Log.Warning("Deserialization returned null for Department ID: {DepartmentId}", departmentId);
                }

                return result.Data;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch Department ID: {DepartmentId}", departmentId);
                return null;
            }
            // var response = await _httpClient.GetAsync($"/api/Departments/{departmentId}");
            // response.EnsureSuccessStatusCode();
            // var content = await response.Content.ReadAsStringAsync();
            // return JsonSerializer.Deserialize<DepartmentDto>(content);
        }
    }
}