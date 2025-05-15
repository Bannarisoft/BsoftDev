using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Commands.Maintenance.PreventiveScheduler;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Hangfire;
using MassTransit;

namespace Core.Application.Consumers.PreventiveScheduler
{
    public class RollBackScheduleWorkOrderConsumer : IConsumer<RollbackPreventiveCommand>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        public RollBackScheduleWorkOrderConsumer(IPreventiveSchedulerCommand preventiveSchedulerCommand, IPreventiveSchedulerQuery preventiveSchedulerQuery)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
        }

        public async Task Consume(ConsumeContext<RollbackPreventiveCommand> context)
        {
            var details =  await _preventiveSchedulerQuery.GetPreventiveSchedulerDetail(context.Message.PreventiveSchedulerHeaderId);
           foreach (var detail in details)
           {
                 if (!string.IsNullOrEmpty(detail.HangfireJobId))
                     {
                         BackgroundJob.Delete(detail.HangfireJobId); 
                     }
           }
           
           
            
            await _preventiveSchedulerCommand.DeleteDetailAsync(context.Message.PreventiveSchedulerHeaderId);
        }
    }
}