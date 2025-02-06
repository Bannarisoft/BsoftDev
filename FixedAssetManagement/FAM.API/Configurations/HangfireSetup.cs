using Hangfire;
using FAM.API.Filters;
namespace FAM.API.Configurations
{
    public static class HangfireSetup
    {
        public static void ConfigureHangfireDashboard(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "BSOFT Hangfire Dashboard",
                Authorization = new[]
                {
                    new HangfireAuthorizationFilter("admin", "basml@1234") // Customize username/password
                }
            });
        }
    }
}