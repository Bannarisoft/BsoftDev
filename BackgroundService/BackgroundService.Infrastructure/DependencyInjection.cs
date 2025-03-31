using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackgroundService.Application.Interfaces;
using BackgroundService.Infrastructure.Configurations;
using BackgroundService.Infrastructure.Services;

namespace BackgroundService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ✅ Correctly bind EmailSettings
            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            services.AddSingleton(emailSettings);

            // Register Services
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();

            return services;
        }
    }
}
