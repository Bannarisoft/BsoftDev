using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.Interfaces.External.IDepartment;
using MaintenanceManagement.Infrastructure.HttpClients.Departments;
using MaintenanceManagement.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.HttpClientPolly;

namespace MaintenanceManagement.Infrastructure
{
    public static class HttpClientFactoryInjection
    {
        public static IServiceCollection AddHttpClientsFactory(this IServiceCollection services, IConfiguration configuration)
        {
            // DepartmentClient
            services.AddHttpClient("DepartmentClient", client =>
            {
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
                client.BaseAddress = new Uri(configuration["HttpClientSettings:UserSessionService"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            // .AddHttpMessageHandler<AuthTokenHandler>()
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
            services.AddScoped<IUserSessionService, UserSessionService>();

              services.AddHttpClient("BackgroundServiceClient", client =>
            {
                client.BaseAddress = new Uri(configuration["HttpClientSettings:BackgroundService"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            // .AddHttpMessageHandler<AuthTokenHandler>()
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            return services;
        }
    }
}