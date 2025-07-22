using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IMaintenance;
using Grpc.Core;
using GrpcServices.Background;
using GrpcServices.HangfireDelete;
using Microsoft.AspNetCore.Http;

namespace MaintenanceManagement.Infrastructure.GrpcClients
{
    public class BackgroundServiceClient : IBackgroundServiceClient
    {
        private readonly MaintenanceJobService.MaintenanceJobServiceClient _grpcClient;
        private readonly MaintenanceHangfireDeleteService.MaintenanceHangfireDeleteServiceClient _hangFiregrpcClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BackgroundServiceClient(MaintenanceJobService.MaintenanceJobServiceClient grpcClient,
        MaintenanceHangfireDeleteService.MaintenanceHangfireDeleteServiceClient hangFiregrpcClient, IHttpContextAccessor httpContextAccessor)
        {
            _grpcClient = grpcClient;
            _hangFiregrpcClient = hangFiregrpcClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> ScheduleWorkOrder(int preventiveScheduleId, int delayInMinutes)
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("No Authorization token found in the current context.");
            }
            //  ✅ Ensure it has "Bearer " prefix
            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {

                token = $"Bearer {token}";
            }

            var metadata = new Metadata
                {
                    { "Authorization", token }
                };
                var callOptions = new CallOptions(metadata);
            var request = new ScheduleWorkOrderRequest
            {
                PreventiveScheduleId = preventiveScheduleId,
                DelayInMinutes = delayInMinutes
            };

            var response = await _grpcClient.ScheduleWorkOrderAsync(request,callOptions);
            return response.JobId ?? string.Empty;
        }
        public  Task<bool> RemoveHangFireJob(string HangfireJobId)
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("No Authorization token found in the current context.");
            }
            //  ✅ Ensure it has "Bearer " prefix
            if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {

                token = $"Bearer {token}";
            }

            var metadata = new Metadata
                {
                    { "Authorization", token }
                };
                var callOptions = new CallOptions(metadata);

            var request = new HangfireRequest
            {
                HangfireJobId = HangfireJobId
            };

            var response =  _hangFiregrpcClient.HangfireRemove(request,callOptions);
            return Task.FromResult(response.IsSuccess);
        }
    }
}