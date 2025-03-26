using Serilog;
using Serilog.Events;
namespace MaintenanceManagement.API.Configurations
{
    public static class SerilogSetup
    {
        public static void ConfigureSerilog(this IHostBuilder hostBuilder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.MongoDB("mongodb://192.168.1.126:27017/Maintenance", 
                    collectionName: "ApplicationLogs", 
                    restrictedToMinimumLevel: LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .CreateLogger();

            hostBuilder.UseSerilog();
        }
    }
}