using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackgroundService.Infrastructure.Configurations;
using Core.Application.Common.Interfaces;
using BackgroundService.Infrastructure.Services;
using Shared.Infrastructure.HttpClientPolly;
using BackgroundService.Application.Interfaces;
using Hangfire;
using Hangfire.SqlServer;
using BackgroundService.Infrastructure.Jobs;
using Polly;
using System.Data;
using Microsoft.Data.SqlClient;


namespace BackgroundService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
           var HangfireConnectionString = configuration.GetConnectionString("HangfireConnection")
                                              .Replace("{SERVER}", Environment.GetEnvironmentVariable("DATABASE_SERVER") ?? "")
                                              .Replace("{USER_ID}", Environment.GetEnvironmentVariable("DATABASE_USERID") ?? "")
                                              .Replace("{ENC_PASSWORD}", Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "");  
            
            if (string.IsNullOrWhiteSpace(HangfireConnectionString))
            {
                throw new InvalidOperationException("Connection string 'HangfireConnectionString' not found or is empty.");
            }

            services.AddTransient<IDbConnection>(sp => new SqlConnection(HangfireConnectionString));

              // Register Hangfire services
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseDefaultTypeSerializer()
                      .UseSqlServerStorage(HangfireConnectionString, new SqlServerStorageOptions
                      {
                          CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                          QueuePollInterval = TimeSpan.Zero,
                          UseRecommendedIsolationLevel = true,
                          UsePageLocksOnDequeue = true,
                          DisableGlobalLocks = true
                      });
            });
            
            // Add the Hangfire server
            services.AddHangfireServer(options => {
            options.Queues = new[] { "schedule_work_order_queue" };
        });
           // ✅ Correctly bind EmailSettings
            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            services.AddSingleton(emailSettings); 

            var smsSettings = new SmsSettings();
            configuration.GetSection("SmsSettings").Bind(smsSettings);
            services.AddSingleton(smsSettings); 

             services.AddHttpClient("UserManagementClient", client =>
            {
                //client.BaseAddress = new Uri("http://localhost:5174"); 
                client.BaseAddress = new Uri(configuration["HttpClientSettings:UserManagementService"]);
                // var userServiceUrl = configuration["HttpClientSettings:UserManagement"];
                // if (string.IsNullOrWhiteSpace(userServiceUrl))
                // {
                //     throw new ArgumentNullException("UserServiceUrl is missing in configuration.");
                // }

                //client.BaseAddress = new Uri(userServiceUrl);
            })
            
               .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)))
            .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));  

             services.AddHttpClient("MaintenanceClient", client =>
            {
                //client.BaseAddress = new Uri("http://localhost:5174"); 
                client.BaseAddress = new Uri(configuration["HttpClientSettings:MaintenanceManagementService"]);
                
            })
            
               .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)))
            .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));  

            services.AddHttpClient(); 
            services.AddScoped<IEmailService, RealEmailService>();
            services.AddScoped<ISmsService, RealSmsService>();
            services.AddScoped<IUserUnlockService, UserUnlockService>();                         
            services.AddTransient<IVerificationCodeCleanupService, VerificationCodeCleanupService>();                           
            services.AddScoped<IUserUnlockBackgroundJob, UserUnlockBackgroundJob>();
            services.AddTransient<IMaintenance, MaintenanceService>();
            
            return services;
    }
    }
}
