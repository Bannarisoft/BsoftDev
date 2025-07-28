using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.Interfaces.IMiscMaster;
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
            int? ApprovalStepDetailId = await _approvalRequestQuery.GetApprovalStepDetailByIdAsync(WorkflowType.Id, context.Message.ModuleTransactionId,context.Message.UnitId,context.Message.DepartmentId);
            var status = await _miscMasterQuery.GetMiscMasterByName(MiscEnumEntity.ApprovalStatus, MiscEnumEntity.Pending);

            if (ApprovalStepDetailId is null)
            {
                throw new InvalidOperationException($"Approval step detail not found");
            }
            var ApprovalReq = new ApprovalRequest
            {
                WorkflowTypeId = WorkflowType.Id,
                ModuleTransactionId = context.Message.ModuleTransactionId,
                ApprovalStepDetailId = ApprovalStepDetailId.Value,
                StatusId = status.Id,
                RequestedDate = DateTimeOffset.Now,
                UnitId = context.Message.UnitId,
                DepartmentId = context.Message.DepartmentId
            };

            await _approvalRequestCommand.CreateAsync(ApprovalReq);
            
        }
    }
}