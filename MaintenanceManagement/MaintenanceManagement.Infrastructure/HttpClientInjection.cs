using System.Net.Http.Headers;
using Core.Application.Common.Interfaces.External.IDepartment;
using MaintenanceManagement.Infrastructure.HttpClients.Departments;
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
                client.BaseAddress = new Uri("http://localhost:5174");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            return services;
        }
    }
}