using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces;

namespace BackgroundService.Infrastructure.Services
{
    public class UserUnlockService  : IUserUnlockService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserUnlockService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> UnlockUser(string userName)
        {
            var client = _httpClientFactory.CreateClient("UserManagementClient");

            var response = await client.PostAsJsonAsync("/api/Auth/unlock", new { UserName = userName });

            return response.IsSuccessStatusCode;
        }
    }
}