using System.Net.Http.Headers;
using System.Text.Json;
using Contracts.Models.Users;
using Polly;
using Polly.Retry;
using SagaOrchestrator.Application.Models;
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
        public async Task<UserDto> GetUserByIdAsync(int userId, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/User/{userId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

                var response = await _retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(request));
                var content = await response.Content.ReadAsStringAsync();

                Log.Information("User API raw response: {Content}", content); // Optional for debugging

                var result = JsonSerializer.Deserialize<UserResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Data == null)
                {
                    Log.Warning("Deserialization returned null for User ID: {UserId}", userId);
                }

                return result?.Data;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to fetch User ID: {UserId}", userId);
                return null;
            }
            // try
            // {
            //     var response = await _retryPolicy.ExecuteAsync(() =>
            //         _httpClient.GetAsync($"/api/User/{userId}")
            //     );

            //     response.EnsureSuccessStatusCode();

            //     var content = await response.Content.ReadAsStringAsync();
            //     var user = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
            //     {
            //         PropertyNameCaseInsensitive = true
            //     });

            //     if (user == null)
            //     {
            //         Log.Warning("Deserialization returned null for User ID: {UserId}", userId);
            //     }

            //     return user;
            // }
            // catch (Exception ex)
            // {
            //     Log.Error(ex, "Failed to fetch User ID: {UserId}", userId);
            //     return null;
            // }
            // var response = await _httpClient.GetAsync($"/api/User/{userId}");
            // response.EnsureSuccessStatusCode();
            // var content = await response.Content.ReadAsStringAsync();
            // return JsonSerializer.Deserialize<UserDto>(content);
        }
    }
}