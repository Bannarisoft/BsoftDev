using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.Interfaces.IMiscMaster;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Application.Workflow.Common.Interfaces.IWorkflowType;
using Contracts.Commands.Workflow;
using MassTransit;
using static BackgroundService.Domain.Common.MiscEnumEntity;

namespace BackgroundService.Application.Consumer.Workflow
{
    public class ApprovalRequestConsumer : IConsumer<CreateApprovalRequestCommand>
    {
        private readonly IWorkflowTypeQuery _workflowTypeQuery;
        private readonly IApprovalRequestCommand _approvalRequestCommand;
        private readonly IApprovalRequestQuery _approvalRequestQuery;
        private readonly IMiscMasterQuery _miscMasterQuery;
        public ApprovalRequestConsumer(IWorkflowTypeQuery workflowTypeQuery, IApprovalRequestCommand approvalRequestCommand,
        IApprovalRequestQuery approvalRequestQuery, IMiscMasterQuery miscMasterQuery)
        {
            _workflowTypeQuery = workflowTypeQuery;
            _approvalRequestCommand = approvalRequestCommand;
            _approvalRequestQuery = approvalRequestQuery;
            _miscMasterQuery = miscMasterQuery;
        }
        public async Task Consume(ConsumeContext<CreateApprovalRequestCommand> context)
        {
            var WorkflowType = await _workflowTypeQuery.GetWorkflowByName(context.Message.ModuleTypeName);
            var ApprovalStepDetailId = await _approvalRequestQuery.GetApprovalStepDetailByIdAsync(WorkflowType.Id, context.Message.ModuleTransactionId);
            var status = await _miscMasterQuery.GetMiscMasterByName(GetApprovalStatus.Status,GetStatusPending.Status);


        }
    }
}