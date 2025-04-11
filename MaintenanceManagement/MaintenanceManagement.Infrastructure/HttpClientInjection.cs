using System.Net.Http.Headers;
using Contracts.Interfaces.IUser;
using Core.Application.Common.Interfaces.External.IDepartment;
using MaintenanceManagement.Infrastructure.HttpClients.Departments;
using MaintenanceManagement.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MaintenanceManagement.Infrastructure
{
    public static class HttpClientInjection
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
            {
                // client.BaseAddress = new Uri("http://localhost:5174");
                client.BaseAddress = new Uri("http://192.168.1.126:81");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddHttpClient<IUserSessionService, UserSessionService>(client =>
            {
                // client.BaseAddress = new Uri("http://localhost:5174/api");
                client.BaseAddress = new Uri("http://192.168.1.126:81/api");
            });

            return services;
        }
    }
}