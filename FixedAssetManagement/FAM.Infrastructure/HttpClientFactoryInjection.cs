using System.Net.Http.Headers;
using Contracts.Interfaces.External.IUser;
using FAM.Infrastructure.HttpClients.Departments;
using FAM.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.HttpClientPolly;

namespace FAM.Infrastructure
{
    public static class HttpClientFactoryInjection
    {
        public static IServiceCollection AddHttpClientsFactory(this IServiceCollection services, IConfiguration configuration)
        {
            // DepartmentClient
            services.AddHttpClient("DepartmentClient", client =>
            {
                //client.BaseAddress = new Uri(configuration["HttpClientSettings:DepartmentService"]);
                client.BaseAddress = new Uri(configuration["HttpClientSettings:DepartmentService"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
             .AddHttpMessageHandler<AuthTokenHandler>()
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
            services.AddScoped<IDepartmentService, DepartmentService>();

            // UserSessionClient
            services.AddHttpClient("UserSessionClient", client =>
            {
                //client.BaseAddress = new Uri(configuration["HttpClientSettings:UserSessionService"]);
                client.BaseAddress = new Uri(configuration["HttpClientSettings:UserSessionService"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            // .AddHttpMessageHandler<AuthTokenHandler>()
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
            services.AddScoped<IUserSessionService, UserSessionService>();

            return services;
        }
    }
}