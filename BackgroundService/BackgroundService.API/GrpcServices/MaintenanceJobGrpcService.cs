using BackgroundService.Application.DelyedJobs;
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
        
        private readonly IMediator _mediator;
        public MaintenanceJobGrpcService(ILogger<MaintenanceJobGrpcService> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            
        }

        public override async Task<ScheduleWorkOrderResponse> ScheduleWorkOrder(ScheduleWorkOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received request to schedule PreventiveScheduleId: {Id}",
                request.PreventiveScheduleId);
            //  var result = await _mediator.Send(new GetJobsQuery());
            //  int delayMin = result.TotalJobs * 2;

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
