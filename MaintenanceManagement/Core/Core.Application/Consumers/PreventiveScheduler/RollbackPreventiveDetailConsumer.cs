using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Commands.Maintenance.PreventiveScheduler;
using Contracts.Interfaces.External.IMaintenance;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Core.Application.Consumers.PreventiveScheduler
{
    public class RollbackPreventiveDetailConsumer : IConsumer<RollbackPreventiveCommand>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly ILogger<RollbackWorkOrderConsumer> _logger;
        private readonly IBackgroundServiceClient _backgroundServiceClient;
        public RollbackPreventiveDetailConsumer(IPreventiveSchedulerCommand preventiveSchedulerCommand, IPreventiveSchedulerQuery preventiveSchedulerQuery,
        ILogger<RollbackWorkOrderConsumer> logger, IBackgroundServiceClient backgroundServiceClient)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _logger = logger;
            _backgroundServiceClient = backgroundServiceClient;
        }

        public async Task Consume(ConsumeContext<RollbackPreventiveCommand> context)
        {
            var existingPreventiveScheduler = await _preventiveSchedulerQuery.GetByIdAsync(context.Message.PreventiveSchedulerHeaderId);
            existingPreventiveScheduler.IsDeleted = Domain.Common.BaseEntity.IsDelete.Deleted;

            await _preventiveSchedulerCommand.DeleteAsync(context.Message.PreventiveSchedulerHeaderId, existingPreventiveScheduler);
            
             var details =  await _preventiveSchedulerQuery.GetPreventiveSchedulerDetail(context.Message.PreventiveSchedulerHeaderId);
             foreach (var detail in details)
             {
                  if (!string.IsNullOrEmpty(detail.HangfireJobId))
                  {
                       _backgroundServiceClient.RemoveHangFireJob(detail.HangfireJobId);
                  }
             }
           
           
            
            await _preventiveSchedulerCommand.DeleteDetailAsync(context.Message.PreventiveSchedulerHeaderId);
        }
    }
}