using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BSOFT.Infrastructure.Logging
{
    public static class SerilogConfiguration
    {
        public static void AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Add Serilog to the .NET Core logging pipeline
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        }
    }
}