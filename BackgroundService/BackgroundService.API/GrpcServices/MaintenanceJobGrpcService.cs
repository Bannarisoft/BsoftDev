using BackgroundService.Application.Interfaces;
using BackgroundService.Application.Jobhistory;
using BackgroundService.Infrastructure.Services;
using Grpc.Core;
using GrpcServices.Background; // This is from generated proto
using Hangfire;
using MediatR;
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

        public override async Task<ScheduleWorkOrderResponse> ScheduleWorkOrder(ScheduleWorkOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received request to schedule PreventiveScheduleId: {Id}",
                request.PreventiveScheduleId);

            string jobId = BackgroundJob.Schedule<MaintenanceService>(
                job => job.SchedulerWorkOrderExecute(request.PreventiveScheduleId),
                TimeSpan.FromMinutes(request.DelayInMinutes)
            );
         
                

            _logger.LogInformation("Scheduled Hangfire Job ID: {JobId}", jobId);

            return new ScheduleWorkOrderResponse
            {
                JobId = jobId
            };
        }
    }
}
