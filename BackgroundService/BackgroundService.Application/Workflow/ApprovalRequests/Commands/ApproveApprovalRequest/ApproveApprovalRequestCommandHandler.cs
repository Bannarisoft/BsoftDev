using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces.IMiscMaster;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Workflow;
using Contracts.Events.Workflow;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Commands.ApproveApprovalRequest
{
    public class ApproveApprovalRequestCommandHandler : IRequestHandler<ApproveApprovalRequestCommand, bool>
    {
        private readonly IMiscMasterQueryRepository _miscMasterQuery;
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IApprovalRequestCommand _approvalRequestCommand;
        private readonly IApprovalRequestQuery _approvalRequestQuery;
        private readonly IEventPublisher _eventPublisher;
        public ApproveApprovalRequestCommandHandler(IMiscMasterQueryRepository miscMasterQuery, IIPAddressService ipAddressService, ITimeZoneService timeZoneService,
            IApprovalRequestCommand approvalRequestCommand, IApprovalRequestQuery approvalRequestQuery, IEventPublisher eventPublisher)
        {
            _miscMasterQuery = miscMasterQuery;
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;
            _approvalRequestCommand = approvalRequestCommand;
            _approvalRequestQuery = approvalRequestQuery;
            _eventPublisher = eventPublisher;
        }
        public async Task<bool> Handle(ApproveApprovalRequestCommand request, CancellationToken cancellationToken)
        {
            int? ApprovalStepDetailId = await _approvalRequestQuery.GetApprovalStepDetailByIdAsync(request.WorkFlowTypeId, request.ModuleTransactionId,request.UnitId,request.DepartmentId);

            var status = await _miscMasterQuery.GetMiscMasterByName(MiscEnumEntity.ApprovalStatus, MiscEnumEntity.Approved);
            string currentIp = _ipAddressService.GetSystemIPAddress();
            int userId = _ipAddressService.GetUserId();
            string username = _ipAddressService.GetUserName();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);

            var ApprovalReq = new ApprovalRequest
            {
                Id = request.Id,
                StatusId = status.Id,
                ModifiedIP = currentIp,
                ModifiedDate = currentTime,
                ModifiedBy = userId,
                ModifiedByName = username
            };
            var result = await _approvalRequestCommand.Approve(ApprovalReq);
            if (ApprovalStepDetailId is not null)
            {
                var correlationId = Guid.NewGuid();
                var @event = new TransactionCreatedEvent
                {
                    CorrelationId = correlationId,
                    ModuleTypeName = request.ModuleTypeName,
                    ModuleTransactionId = request.ModuleTransactionId,
                    UnitId = request.UnitId,
                    DepartmentId = request.DepartmentId
                };
                
                await _eventPublisher.SaveEventAsync(@event);
                await _eventPublisher.PublishPendingEventsAsync();
            }
             

            return result;          
        }
    }
}