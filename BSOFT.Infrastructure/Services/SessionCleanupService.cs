using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserSession;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BSOFT.Infrastructure.Services
{
    public class SessionCleanupService  : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public SessionCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var sessionRepository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

                // Deactivate expired sessions
                await sessionRepository.DeactivateExpiredSessionsAsync();
            }

            // Run cleanup every 10 minutes
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }
}
}