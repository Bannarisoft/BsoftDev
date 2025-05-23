using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IMaintenance;
using GrpcServices.Background;

namespace MaintenanceManagement.Infrastructure.GrpcClients
{
    public class BackgroundServiceClient : IBackgroundServiceClient
    {
        private readonly MaintenanceJobService.MaintenanceJobServiceClient _grpcClient;

        public BackgroundServiceClient(MaintenanceJobService.MaintenanceJobServiceClient grpcClient)
        {
            _grpcClient = grpcClient;
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
    }
}