using Contracts.Interfaces.IUser;
using FAM.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.HttpClientPolly;

namespace FAM.Infrastructure
{
    public static class HttpClientInjection
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            // Named HttpClient with Polly policies
            // UserSessionClient
            services.AddHttpClient("UserSessionClient", client =>
            {
                client.BaseAddress = new Uri(configuration["HttpClientSettings:UserSessionService"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })

            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
            // Manual registration of service that uses IHttpClientFactory
            services.AddScoped<IUserSessionService, UserSessionService>();

            return services;
        }
       
    }
}