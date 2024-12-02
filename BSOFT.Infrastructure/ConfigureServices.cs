using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace BSOFT.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices
            (this IServiceCollection services, IConfiguration Configuration)
            {
                return services;
            }
    }
}