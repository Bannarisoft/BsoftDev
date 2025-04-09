using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Contracts.Interfaces.IUser;
using Contracts.Models.Common;
using Contracts.Models.Users;

namespace FAM.Infrastructure.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly HttpClient _httpClient;
        public UserSessionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<UserSessionDto?> GetSessionByJwtIdAsync(string jwtId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/Auth/session/{jwtId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserSessionDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return apiResponse?.Data;
        }

        public async Task<bool> UpdateSessionAsync(string jwtId, DateTimeOffset lastActivity, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                      new AuthenticationHeaderValue("Bearer", token);

            var payload = new { LastActivity = lastActivity };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/Auth/session/{jwtId}/activity", content);
            if (!response.IsSuccessStatusCode)
                return false;

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return apiResponse?.Data ?? false;
        }
    }
}