using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace BSOFT.Infrastructure.Resilience
{
    public static class PollyPolicies
    {
        public static IServiceCollection AddPollyPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            // Get timeout value from configuration
            int timeoutInSeconds = configuration.GetValue<int>("Polly:TimeoutSeconds", 10);

            // Combine policies into a single PolicyWrap
            var policyWrap = Policy.WrapAsync(
                GetFallbackPolicy(),
                GetRateLimitPolicy(),
                // GetCachePolicy(memoryCacheProvider),
                GetBulkheadPolicy(),
                GetTimeoutPolicy(timeoutInSeconds),
                GetRetryPolicy(),
                GetCircuitBreakerPolicy()
            );

            // Attach the PolicyWrap to the HTTP client
            services.AddHttpClient("ResilientHttpClient")
                .AddPolicyHandler(policyWrap);

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Handles 5xx, 408, and network issues
                .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests) // Retry on 429
                .WaitAndRetryAsync(
                    3, // Retry 3 times
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    (outcome, timespan, retryAttempt, context) =>
                    {
                        Log.Warning(
                            "Retry attempt {RetryAttempt} after {Delay}s due to {Reason}",
                            retryAttempt,
                            timespan.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    2, // Break after 2 consecutive failures
                    TimeSpan.FromSeconds(30), // Duration of the break
                    onBreak: (result, duration) =>
                    {
                        Log.Error(
                            "Circuit breaker opened for {Duration}s due to: {Reason}",
                            duration.TotalSeconds,
                            result.Exception?.Message ?? result.Result.StatusCode.ToString());
                    },
                    onReset: () => Log.Information("Circuit breaker closed."),
                    onHalfOpen: () => Log.Information("Circuit breaker is half-open."));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(int timeoutInSeconds)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(timeoutInSeconds, Polly.Timeout.TimeoutStrategy.Pessimistic,
                (context, timespan, task) =>
                {
                    Log.Warning("Request timed out after {Timeout}s.", timespan.TotalSeconds);
                    return Task.CompletedTask;
                });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
        {
            return Policy<HttpResponseMessage>
                .Handle<Exception>()
                .FallbackAsync(
                    new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent("Fallback response due to an error.")
                    },
                    onFallbackAsync: (exception, context) =>
                    {
                        Log.Error("Fallback executed due to: {Exception}", exception.Exception.Message);
                        return Task.CompletedTask;
                    });
        }
        private static IAsyncPolicy<HttpResponseMessage> GetRateLimitPolicy()
        {
            return Policy.RateLimitAsync<HttpResponseMessage>(
                numberOfExecutions: 10, // Allow 10 requests
                perTimeSpan: TimeSpan.FromSeconds(1), // Per second
                maxBurst: 2 // Queue 2 requests before rejecting
            );
        }

        // private static IAsyncPolicy<HttpResponseMessage> GetCachePolicy(MemoryCacheProvider memoryCacheProvider)
        // {
        //     return Policy.CacheAsync(
        //         cacheProvider: memoryCacheProvider,
        //         ttl: TimeSpan.FromMinutes(5), // Cache TTL
        //         onCacheError: (context, key, exception) =>
        //         {
        //             Log.Warning("Cache error for key {Key}: {Error}", key, exception.Message);
        //         });
        // }

        private static IAsyncPolicy<HttpResponseMessage> GetBulkheadPolicy()
        {
            return Policy.BulkheadAsync<HttpResponseMessage>(
                maxParallelization: 5, // Allow 5 concurrent requests
                maxQueuingActions: 10, // Allow 10 requests in the queue
                onBulkheadRejectedAsync: context =>
                {
                    Log.Warning("Bulkhead limit exceeded. Request rejected.");
                    return Task.CompletedTask;
                });
        }
    }
}
