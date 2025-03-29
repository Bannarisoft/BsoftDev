using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts.Models.Users;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;

namespace SagaOrchestrator.Application.Orchestration.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/User/{userId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserDto>(content);
        }
    }
}