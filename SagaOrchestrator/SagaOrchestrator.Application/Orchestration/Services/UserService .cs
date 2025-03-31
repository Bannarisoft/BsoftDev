using System.Text.Json;
using Contracts.Models.Users;
using Polly;
using Polly.Retry;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;
using Serilog;

namespace SagaOrchestrator.Application.Orchestration.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .RetryAsync(3);
        }
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"/api/User/{userId}")
                );

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserDto>(content);
            }
            catch (Exception ex)
            {
                Log.Information($"Failed to fetch User ID: {userId}. Error: {ex.Message}");
                return null;
            }
            // var response = await _httpClient.GetAsync($"/api/User/{userId}");
            // response.EnsureSuccessStatusCode();
            // var content = await response.Content.ReadAsStringAsync();
            // return JsonSerializer.Deserialize<UserDto>(content);
        }
    }
}