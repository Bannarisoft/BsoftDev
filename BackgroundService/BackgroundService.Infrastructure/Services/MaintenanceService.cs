using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces;
using Dapper;
using Hangfire;

namespace BackgroundService.Infrastructure.Services
{
    [Queue("schedule_work_order_queue")]
    public class MaintenanceService : IMaintenance
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDbConnection _dbConnection;
        public MaintenanceService(IHttpClientFactory httpClientFactory, IDbConnection dbConnection)
        {
            _httpClientFactory = httpClientFactory;
            _dbConnection = dbConnection;
        }

        public async Task SchedulerWorkOrderExecute(int PreventiveScheduleId)
        {
            var client = _httpClientFactory.CreateClient("MaintenanceClient");
            await client.PostAsJsonAsync("api/PreventiveScheduler/HangfireSchedule", new { PreventiveScheduleId = PreventiveScheduleId });
        }
        public async Task<int> GetTotalPendingJobsAsync()
         {
             const string sql = @"
                 SELECT COUNT(*) 
                 FROM Hangfire.Job 
                 WHERE StateName IN ('Enqueued', 'Scheduled');";
        
             return await _dbConnection.ExecuteScalarAsync<int>(sql);
         }

    }
}