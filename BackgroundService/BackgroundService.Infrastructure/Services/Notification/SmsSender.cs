using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BackgroundService.Application.Interfaces;
using BackgroundService.Infrastructure.Configurations;
using BackgroundService.Application.Interfaces.Notification;

namespace BackgroundService.Infrastructure.Services.Notification
{
    public class SmsSender : ISmsSender
    {
        private readonly SmsSettings _smsSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SmsSender> _logger;

        public SmsSender(SmsSettings smsSettings, IHttpClientFactory httpClientFactory, ILogger<SmsSender> logger)
        {
            _smsSettings = smsSettings;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<bool> SendSmsAsync(List<string> mobileNumbers, string message)
        {
             try
            {
                if (mobileNumbers == null || !mobileNumbers.Any())
                {
                    _logger.LogWarning("⚠️ No mobile numbers provided. SMS aborted.");
                    return false;
                }                

                foreach (var number in mobileNumbers)
                {
                    var success = await SendSmsToSingleAsync(number.ToString(), message);

                    if (!success)
                    {
                            _logger.LogInformation("📱 Sending SMS to {Number}: {Message}", number, message);
                            await Task.Delay(50); // Simulate API delay
                    }
                }
               return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send SMS.");
                return false;
            }
        }
        // ✅ This is the helper method to send SMS to a single user
        private async Task<bool> SendSmsToSingleAsync(string phoneNumber, string message)
        {
            try
            {
                var url = $"{_smsSettings.BaseUrl}?apikey={_smsSettings.ApiKey}&route={_smsSettings.Route}&sender={_smsSettings.Sender}&number={phoneNumber}&message={Uri.EscapeDataString(message)}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("SMS sending failed to {Phone}: {StatusCode}", phoneNumber, response.StatusCode);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while sending SMS to {Phone}", phoneNumber);
                return false;
            }
        }
    }
}
