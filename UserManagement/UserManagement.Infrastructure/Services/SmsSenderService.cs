
using System.Net.Http.Json;
using Core.Application.Common.Interfaces;
using Contracts.Events.Notifications;


namespace UserManagement.Infrastructure.Services
{
    public class SmsSenderService : ISmsService
    {        
        private readonly IHttpClientFactory _httpClientFactory;

        public SmsSenderService(IHttpClientFactory httpClientFactory)
        {            
            _httpClientFactory = httpClientFactory; 
        }

        public async Task<bool> SendSmsAsync(SendSmsCommand command)
        {
            var client = _httpClientFactory.CreateClient("BackgroundServiceClient");
            var response = await client.PostAsJsonAsync("api/sms/send", command);
            return response.IsSuccessStatusCode;
        }
    }
}
