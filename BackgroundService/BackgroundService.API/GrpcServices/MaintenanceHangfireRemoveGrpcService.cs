using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcServices.HangfireDelete;
using Hangfire;

namespace BackgroundService.API.GrpcServices
{
    public class MaintenanceHangfireRemoveGrpcService : MaintenanceHangfireDeleteService.MaintenanceHangfireDeleteServiceBase
    {

        public override Task<HangfireResponse> HangfireRemove(HangfireRequest request, ServerCallContext context)
        {

            if (!string.IsNullOrEmpty(request.HangfireJobId))
            {
                BackgroundJob.Delete(request.HangfireJobId);
            }
            
                var response = new HangfireResponse
               {
                   IsSuccess = true
               };

                return Task.FromResult(response);
            
        }
        
    }
}