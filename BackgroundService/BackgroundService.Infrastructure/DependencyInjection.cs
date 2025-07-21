using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackgroundService.Infrastructure.Configurations;
using BackgroundService.Infrastructure.Services;
using BackgroundService.Application.Interfaces;
using Hangfire;
using Hangfire.SqlServer;
using BackgroundService.Infrastructure.Jobs;
using Polly;
using System.Data;
using BackgroundService.Infrastructure.Repositories.HangFire;
using BackgroundService.Application.Common.Notification.Interfaces;
using BackgroundService.Infrastructure.Repositories.Notification;
using BackgroundService.Infrastructure.Data.Notification;
using Microsoft.EntityFrameworkCore;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationConfig;
using BackgroundService.Infrastructure.Repositories.Notification.NotificationConfig;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroup;
using BackgroundService.Infrastructure.Repositories.Notification.NotificationGroup;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Infrastructure.Repositories.Notification.NotificationLevelHierarchy;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationTemplate;
using BackgroundService.Infrastructure.Repositories.Notification.NotificationTemplate;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Infrastructure.Repositories.Notification.NotificationGroupMember;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Infrastructure.Repositories.Notification.NotificationEventRules;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Application.Notification.Common.Mappings;
using MassTransit;
using BackgroundService.Application.Consumers;
using BackgroundService.Application.Interfaces.Notification;
using BackgroundService.Infrastructure.Services.Notification;
using BackgroundService.Application.Notification;
using Contracts.Events.Notifications.WorkOrder.Sms;
using Contracts.Events.Notifications.WorkOrder.Email;
using Contracts.Events.Notifications.WorkOrder.InApp;

namespace BackgroundService.Infrastructure
{
    public static class DependencyInjection
    {
        private static readonly string[] HangfireQueues = ["schedule_work_order_queue","forgot_password_queue","user_unlock_queue"];
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IServiceCollection builder)
        {
            var HangfireConnectionString = configuration.GetConnectionString("HangfireConnection")
                                               .Replace("{SERVER}", Environment.GetEnvironmentVariable("DATABASE_SERVER") ?? "")
                                               .Replace("{USER_ID}", Environment.GetEnvironmentVariable("DATABASE_USERID") ?? "")
                                               .Replace("{ENC_PASSWORD}", Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "");

             var NotificationConnectionString = configuration.GetConnectionString("NotificationConnection")
                                               .Replace("{SERVER}", Environment.GetEnvironmentVariable("DATABASE_SERVER") ?? "")
                                               .Replace("{USER_ID}", Environment.GetEnvironmentVariable("DATABASE_USERID") ?? "")
                                               .Replace("{ENC_PASSWORD}", Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "");

            if (string.IsNullOrWhiteSpace(HangfireConnectionString))
            {
                throw new InvalidOperationException("Connection string 'HangfireConnectionString' not found or is empty.");
            }
            else if (string.IsNullOrWhiteSpace(NotificationConnectionString))
            {
                throw new InvalidOperationException("Connection string 'NotificationConnectionString' not found or is empty.");
            }

            services.AddTransient<IHangfireDbConnectionFactory>(sp => new HangfireDbConnectionFactory(HangfireConnectionString));
            services.AddTransient<INotificationDbConnectionFactory>(sp => new NotificationDbConnectionFactory(NotificationConnectionString));
            services.AddScoped<IDbConnection>(sp =>
            {
                var factory = sp.GetRequiredService<INotificationDbConnectionFactory>();
                return factory.CreateConnection();
            });
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

            services.AddDbContext<NotificationDbContext>(options =>
                options.UseSqlServer(NotificationConnectionString));

            // Add the Hangfire server
            services.AddHangfireServer(options =>
            {
                options.ServerName = configuration["HangfireServer:Server"];
                options.Queues = HangfireQueues;
            });
            //Notification
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddConsumer<ResolveNotificationChannelsConsumer>();
                x.AddConsumer<SendEmailNotificationConsumer>();
                x.AddConsumer<SendSmsNotificationConsumer>();
                x.AddConsumer<SendInAppNotificationConsumer>();
                

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                

                    cfg.ReceiveEndpoint("resolve-notification-channels-queue", e =>
                    {
                        e.ConfigureConsumer<ResolveNotificationChannelsConsumer>(context);
                         
                    });
             
                    cfg.ReceiveEndpoint("email-notification-queue", e =>
                    {                        
                        e.Bind("Contracts.Events.Notifications.WorkOrder.Email:SendEmailNotificationInternalCommand", s =>
                        {
                            s.ExchangeType = "fanout"; // Required if you're using fanout-based exchange
                        });

                        e.ConfigureConsumer<SendEmailNotificationConsumer>(context);
                    });
                    cfg.ReceiveEndpoint("sms-notification-queue", e =>
                    {
                        e.Bind("Contracts.Events.Notifications.WorkOrder.Sms:SendSmsNotificationInternalCommand", s =>
                        {
                            s.ExchangeType = "fanout"; // Required if you're using fanout-based exchange
                        });

                        e.ConfigureConsumer<SendSmsNotificationConsumer>(context);
                    });  cfg.ReceiveEndpoint("inapp-notification-queue", e =>
                    {
                        e.Bind("Contracts.Events.Notifications.WorkOrder.InApp:SendInAppNotificationInternalCommand", s =>
                        {
                            s.ExchangeType = "fanout"; // Required if you're using fanout-based exchange
                        });

                        e.ConfigureConsumer<SendInAppNotificationConsumer>(context);
                    });
                     
                });
            });

            services.AddMassTransitHostedService();

            
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

            services.AddAutoMapper(typeof(NotificationEventRuleProfile));

            services.AddHttpClient();
            services.AddScoped<IEmailService, RealEmailService>();
            services.AddScoped<ISmsService, RealSmsService>();
            services.AddScoped<IUserUnlockService, UserUnlockService>();
            services.AddTransient<IVerificationCodeCleanupService, VerificationCodeCleanupService>();
            services.AddScoped<IUserUnlockBackgroundJob, UserUnlockBackgroundJob>();
            services.AddTransient<IMaintenance, MaintenanceService>();
            services.AddScoped<INotificationConfigCommandRepository, NotificationConfigCommandRepository>();  
            services.AddScoped<INotificationConfigQueryRepository, NotificationConfigQueryRepository>();  
            services.AddScoped<INotificationGroupCommand, NotificationGroupCommandRepository >();
            services.AddScoped<INotificationGroupQuery, NotificationGroupQueryRepository >();
            services.AddScoped<IIPAddressService, IPAddressService>();
            services.AddSingleton<ITimeZoneService, TimeZoneService>();
            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();   
            services.AddScoped<INotificationLevelHierarchyCommandRepository, NotificationLevelHierarchyCommandRepository>();  
            services.AddScoped<INotificationLevelHierarchyQueryRepository, NotificationLevelHierarchyQueryRepository>();  
            services.AddScoped<INotificationTemplateCommandRepository, NotificationTemplateCommandRepository>();  
            services.AddScoped<INotificationTemplateQueryRepository, NotificationTemplateQueryRepository>();
            services.AddScoped<INotificationUserResolver, NotificationUserResolver>();
            services.AddScoped<NotificationResolverHandler>();
            //Notification
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<IInAppNotifier, InAppNotifier>(); 
            services.AddScoped<INotificationGroupMemberCommand, NotificationGroupMemberCommandRepository >();
            services.AddScoped<INotificationGroupMemberQuery, NotificationGroupMemberQueryRepository >();
            services.AddScoped<INotificationEventRuleCommand, NotificationEventRuleCommandRepository >();
            services.AddScoped<INotificationEventRuleQuery, NotificationEventRuleQueryRepository >();
            services.AddScoped<INotificationLogger, NotificationLogger>();
            return services;
        }
    }
}
