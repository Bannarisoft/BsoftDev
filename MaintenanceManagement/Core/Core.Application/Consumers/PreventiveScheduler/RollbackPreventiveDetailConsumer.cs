using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Commands.Maintenance.PreventiveScheduler;
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
        public RollbackPreventiveDetailConsumer(IPreventiveSchedulerCommand preventiveSchedulerCommand, IPreventiveSchedulerQuery preventiveSchedulerQuery,ILogger<RollbackWorkOrderConsumer> logger)
        {
            _preventiveSchedulerCommand =preventiveSchedulerCommand;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RollbackPreventiveCommand> context)
        {
            var existingPreventiveScheduler = await _preventiveSchedulerQuery.GetByIdAsync(context.Message.PreventiveSchedulerHeaderId);
            existingPreventiveScheduler.IsDeleted = Domain.Common.BaseEntity.IsDelete.Deleted;
           
            await _preventiveSchedulerCommand.DeleteAsync(context.Message.PreventiveSchedulerHeaderId,existingPreventiveScheduler);
        }
    }
}