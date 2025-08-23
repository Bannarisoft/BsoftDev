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
            // var WorkflowType = await _workflowTypeQuery.GetWorkflowByName(context.Message.ModuleTypeName);
        //     List<int> ApprovalStepDetailId = await _approvalRequestQuery.GetApprovalStepDetailByIdAsync(context.Message.ModuleTypeName, context.Message.ModuleTransactionId,context.Message.UnitId,context.Message.DepartmentId);
        //     var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(context.Message.Payload);
        //     List<int> ActualApprovalStepDetailId = await _approvalRequestQuery.StartApprovalProcessAsync(ApprovalStepDetailId, requestData);
        //     var status = await _miscMasterQuery.GetMiscMasterByName(MiscEnumEntity.ApprovalStatus, MiscEnumEntity.Pending);

        //     if (ActualApprovalStepDetailId is null)
        //     {
        //         throw new InvalidOperationException($"Approval step detail not found");
        //     }
        //    var approvalRequests = new List<ApprovalRequest>();

        //     foreach (var id in ActualApprovalStepDetailId)
        //     {
        //         approvalRequests.Add(new ApprovalRequest
        //         {
        //             WorkflowType = context.Message.ModuleTypeName,
        //             ModuleTransactionId = context.Message.ModuleTransactionId,
        //             ApprovalStepDetailId = id,
        //             StatusId = status.Id,
        //             RequestedDate = DateTimeOffset.Now,
        //             UnitId = context.Message.UnitId,
        //             DepartmentId = context.Message.DepartmentId,
        //             Action ="test"
        //         });
        //     }

            await _approvalRequestCommand.CreateBulkAsync(context.Message.ModuleTypeName,context.Message.ModuleTransactionId,context.Message.Payload);
            
        }
    }
}