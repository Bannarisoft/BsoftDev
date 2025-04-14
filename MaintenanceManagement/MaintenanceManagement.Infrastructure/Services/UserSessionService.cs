using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Contracts.Interfaces.IUser;
using Contracts.Models.Common;
using Contracts.Models.Users;

namespace MaintenanceManagement.Infrastructure.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public UserSessionService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<UserSessionDto?> GetSessionByJwtIdAsync(string jwtId, string token)
        {
            var client = _httpClientFactory.CreateClient("UserSessionClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/Auth/session/{jwtId}");
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
            var client = _httpClientFactory.CreateClient("UserSessionClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var sessionUpdate = new { JwtId = jwtId, LastActivity = lastActivity };
            var response = await client.PutAsJsonAsync($"api/Auth/session/{jwtId}", sessionUpdate);

            return response.IsSuccessStatusCode;
        }
    }
}