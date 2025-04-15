using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;
using SagaOrchestrator.Application.Orchestration.Services.MaintenanceServices;
using SagaOrchestrator.Application.Orchestration.Services.UserServices;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SagaOrchestrator.Infrastructure.PollyResilience
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
        {
            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri("http://192.168.1.126:81");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
            {
                client.BaseAddress = new Uri("http://192.168.1.126:81");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"Retrying... Attempt {retryAttempt}");
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (result, timespan) =>
                    {
                        Console.WriteLine("Circuit broken!");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Circuit reset!");
                    });
        }
    }
}
