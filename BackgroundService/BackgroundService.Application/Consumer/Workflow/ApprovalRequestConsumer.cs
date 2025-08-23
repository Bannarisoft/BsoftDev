using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces.IMiscMaster;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Application.Workflow.Common.Interfaces.IWorkflowType;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Workflow;
using Contracts.Commands.Workflow;
using MassTransit;


namespace BackgroundService.Application.Consumer.Workflow
{
    public class ApprovalRequestConsumer : IConsumer<CreateApprovalRequestCommand>
    {
        private readonly IWorkflowTypeQuery _workflowTypeQuery;
        private readonly IApprovalRequestCommand _approvalRequestCommand;
        private readonly IApprovalRequestQuery _approvalRequestQuery;
        private readonly IMiscMasterQueryRepository _miscMasterQuery;
        public ApprovalRequestConsumer(IWorkflowTypeQuery workflowTypeQuery, IApprovalRequestCommand approvalRequestCommand,
        IApprovalRequestQuery approvalRequestQuery, IMiscMasterQueryRepository miscMasterQuery)
        {
            _workflowTypeQuery = workflowTypeQuery;
            _approvalRequestCommand = approvalRequestCommand;
            _approvalRequestQuery = approvalRequestQuery;
            _miscMasterQuery = miscMasterQuery;
        }
        public async Task Consume(ConsumeContext<CreateApprovalRequestCommand> context)
        {
          

            await _approvalRequestCommand.CreateBulkAsync(context.Message.ModuleTypeName,context.Message.ModuleTransactionId,context.Message.Payload);
            
        }
    }
}