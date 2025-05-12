
using Core.Application.Common.Interfaces;
using Contracts.Events.Notifications;
using System.Net.Http.Json;

namespace UserManagement.Infrastructure.Services
{
    public class BackgroundServiceClient  : IBackgroundServiceClient
    {
    private readonly IHttpClientFactory _httpClientFactory;

        public BackgroundServiceClient (IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task UserUnlock(string userName, int delayInMinutes)
        {
             var client = _httpClientFactory.CreateClient("BackgroundServiceClient");

            var removeCodeCommand = new ScheduleRemoveCodeCommand
            {
                UserName = userName,
                DelayInMinutes = delayInMinutes
            };
            var response = await client.PostAsJsonAsync("/api/userhangfirejobs/user-unlock", removeCodeCommand);
            response.EnsureSuccessStatusCode();
        }

        public async Task ScheduleVerificationCodeCleanupAsync(string userName, int delayInMinutes)
        {
            var client = _httpClientFactory.CreateClient("BackgroundServiceClient");

            var removeCodeCommand = new ScheduleRemoveCodeCommand
            {
                UserName = userName,
                DelayInMinutes = delayInMinutes
            };
            var response = await client.PostAsJsonAsync("/api/userhangfirejobs/user-verification-code-removal", removeCodeCommand);
            response.EnsureSuccessStatusCode();
        }      
    }   

}