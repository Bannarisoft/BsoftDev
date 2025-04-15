using System.Net.Http.Headers;
using Contracts.Interfaces.IUser;
using Core.Application.Common.Interfaces.External.IDepartment;
using MaintenanceManagement.Infrastructure.HttpClients.Departments;
using MaintenanceManagement.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Shared.Infrastructure.HttpClientPolly;

namespace MaintenanceManagement.Infrastructure
{
    public static class HttpClientInjection
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            // DepartmentClient
            services.AddHttpClient("DepartmentClient", client =>
            {
                client.BaseAddress = new Uri(configuration["HttpClientSettings:DepartmentService"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
            services.AddScoped<IDepartmentService, DepartmentService>();

            // UserSessionClient
            services.AddHttpClient("UserSessionClient", client =>
            {
                client.BaseAddress = new Uri(configuration["HttpClientSettings:UserSessionService"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })

            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
            services.AddScoped<IUserSessionService, UserSessionService>();

            return services;
        }
    }
}