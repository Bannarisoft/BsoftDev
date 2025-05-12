using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces;

namespace BackgroundService.Infrastructure.Services
{
    public class MaintenanceService : IMaintenance
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MaintenanceService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SchedulerWorkOrderExecute(int PreventiveScheduleId)
        {
             var client = _httpClientFactory.CreateClient("MaintenanceClient");
            await client.PostAsJsonAsync("api/PreventiveScheduler/HangfireSchedule", new { PreventiveScheduleId = PreventiveScheduleId });
        }

    }
}