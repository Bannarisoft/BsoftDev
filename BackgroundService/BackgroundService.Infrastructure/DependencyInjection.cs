using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BackgroundService.Infrastructure.Configurations;
using System.Reflection;
using Core.Application.Common.Interfaces;
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

            var smsSettings = new SmsSettings();
            configuration.GetSection("SmsSettings").Bind(smsSettings);
            services.AddSingleton(smsSettings); 

          services.AddHttpClient();  
           //services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            //services.Configure<SmsSettings>(configuration.GetSection("SmsSettings"));

          services.AddScoped<IEmailService, RealEmailService>();
          services.AddScoped<ISmsService, RealSmsService>();
        return services;
    }
    }
}
