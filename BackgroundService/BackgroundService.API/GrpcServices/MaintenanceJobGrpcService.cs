using BackgroundService.Application.DelyedJobs;
using BackgroundService.Application.Interfaces;
using Grpc.Core;
using GrpcServices.Background; // This is from generated proto
using Hangfire;
using Microsoft.Extensions.Logging;

namespace BackgroundService.API.GrpcServices
{
    public class MaintenanceJobGrpcService : MaintenanceJobService.MaintenanceJobServiceBase
    {
        private readonly ILogger<MaintenanceJobGrpcService> _logger;

        public MaintenanceJobGrpcService(ILogger<MaintenanceJobGrpcService> logger)
        {
            _logger = logger;
        }

        public override Task<ScheduleWorkOrderResponse> ScheduleWorkOrder(ScheduleWorkOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received request to schedule PreventiveScheduleId: {Id}",
                request.PreventiveScheduleId);

            // Schedule Hangfire Job
            string jobId = BackgroundJob.Schedule<UserUnlockservice>(
                job => job.ScheduleworkOrderExecute(request.PreventiveScheduleId),
                TimeSpan.FromMinutes(request.DelayInMinutes)
            );

            _logger.LogInformation("Scheduled Hangfire Job ID: {JobId}", jobId);

            return Task.FromResult(new ScheduleWorkOrderResponse
            {
                JobId = jobId
            });
        }
    }
}
