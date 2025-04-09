using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;
using SagaOrchestrator.Application.Orchestration.Services.MaintenanceServices;
using SagaOrchestrator.Application.Orchestration.Services.UserServices;

namespace SagaOrchestrator.Infrastructure.PollyResilience
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
        {
            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5174"); // URL of UserManagement API
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            // HttpClient for IUserSessionService
            services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5174"); // Same or different depending on your setup
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}