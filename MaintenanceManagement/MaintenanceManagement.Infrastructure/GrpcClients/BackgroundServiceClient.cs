using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IMaintenance;
using GrpcServices.Background;
using GrpcServices.HangfireDelete;

namespace MaintenanceManagement.Infrastructure.GrpcClients
{
    public class BackgroundServiceClient : IBackgroundServiceClient
    {
        private readonly MaintenanceJobService.MaintenanceJobServiceClient _grpcClient;
        private readonly MaintenanceHangfireDeleteService.MaintenanceHangfireDeleteServiceClient _hangFiregrpcClient;

        public BackgroundServiceClient(MaintenanceJobService.MaintenanceJobServiceClient grpcClient,
        MaintenanceHangfireDeleteService.MaintenanceHangfireDeleteServiceClient hangFiregrpcClient)
        {
            _grpcClient = grpcClient;
            _hangFiregrpcClient = hangFiregrpcClient;
        }

        public async Task<string> ScheduleWorkOrder(int preventiveScheduleId, int delayInMinutes)
        {
            var request = new ScheduleWorkOrderRequest
            {
                PreventiveScheduleId = preventiveScheduleId,
                DelayInMinutes = delayInMinutes
            };

            var response = await _grpcClient.ScheduleWorkOrderAsync(request);
            return response.JobId ?? string.Empty;
        }
        public  Task<bool> RemoveHangFireJob(string HangfireJobId)
        {
            var request = new HangfireRequest
            {
                HangfireJobId = HangfireJobId
            };

            var response =  _hangFiregrpcClient.HangfireRemove(request);
            return Task.FromResult(response.IsSuccess);
        }
    }
}