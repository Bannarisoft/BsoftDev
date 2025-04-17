
using System.Net.Http.Json;
using Core.Application.Common.Interfaces;
using Contracts.Events.Notifications;

public class EmailSenderService  : IEmailService
{
   private readonly IHttpClientFactory _httpClientFactory;

    public EmailSenderService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> SendEmailAsync(SendEmailCommand command)
    {
        var client = _httpClientFactory.CreateClient("BackgroundServiceClient");
        var response = await client.PostAsJsonAsync("api/email/send", command);
        return response.IsSuccessStatusCode;
    }
}    

