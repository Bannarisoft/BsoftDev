using System;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Domain.Common;
using Microsoft.Extensions.Options;

namespace BSOFT.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private readonly SmsSettings _smsSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public SmsService(IOptions<SmsSettings> smsSettings, IHttpClientFactory httpClientFactory)
        {
            _smsSettings = smsSettings.Value;
            _httpClientFactory = httpClientFactory; // ✅ Ensure HttpClientFactory is injected
        }

        public async Task<bool> SendSmsAsync(string to, string message)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(); // ✅ Use HttpClientFactory

                string requestUrl = $"{_smsSettings.BaseUrl}?key={_smsSettings.ApiKey}&route={_smsSettings.Route}" +
                                    $"&sender={_smsSettings.Sender}&number={to}&sms={Uri.EscapeDataString(message)}" +
                                    $" If you didn't request this,please contact us at {_smsSettings.adminmailid} -BASML"+
                                    $"&templateid={_smsSettings.TemplateId}";

                HttpResponseMessage response = await client.GetAsync(requestUrl);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                return false;
            }
        }
    }
}
