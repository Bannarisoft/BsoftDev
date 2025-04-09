using System.Net.Http.Headers;
using Contracts.Interfaces.IUser;
using FAM.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FAM.Infrastructure
{
    public static class HttpClientInjection
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            // HttpClient Registration using HttpClientFactory
            services.AddHttpClient<IUserSessionService, UserSessionService>(client =>
            {
                // client.BaseAddress = new Uri("http://localhost:5174/api");
                client.BaseAddress = new Uri("http://192.168.1.126:81/api");
            });

            return services;
        }
    }
}