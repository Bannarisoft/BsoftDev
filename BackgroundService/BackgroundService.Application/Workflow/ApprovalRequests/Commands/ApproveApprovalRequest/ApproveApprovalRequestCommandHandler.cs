using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Interfaces.IMiscMaster;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Workflow;
using Contracts.Dtos.Purchase;
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
        private readonly IMapper _imapper;
        public ApproveApprovalRequestCommandHandler(IMiscMasterQueryRepository miscMasterQuery, IIPAddressService ipAddressService, ITimeZoneService timeZoneService,
            IApprovalRequestCommand approvalRequestCommand, IApprovalRequestQuery approvalRequestQuery, IEventPublisher eventPublisher, IMapper imapper)
        {
            _miscMasterQuery = miscMasterQuery;
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;
            _approvalRequestCommand = approvalRequestCommand;
            _approvalRequestQuery = approvalRequestQuery;
            _eventPublisher = eventPublisher;
            _imapper = imapper;
        }
        public async Task<bool> Handle(ApproveApprovalRequestCommand request, CancellationToken cancellationToken)
        {
            // int? ApprovalStepDetailId = await _approvalRequestQuery.GetApprovalStepDetailByIdAsync(request.WorkFlowTypeId, request.ModuleTransactionId,request.UnitId,request.DepartmentId);

            var statusApproved = await _miscMasterQuery.GetMiscMasterByName(MiscEnumEntity.ApprovalStatus, MiscEnumEntity.Approved);
            var statusRejected = await _miscMasterQuery.GetMiscMasterByName(MiscEnumEntity.ApprovalStatus, MiscEnumEntity.Rejected);
            var statusPending = await _miscMasterQuery.GetMiscMasterByName(MiscEnumEntity.ApprovalStatus, MiscEnumEntity.Pending);
            string currentIp = _ipAddressService.GetSystemIPAddress();
            int userId = _ipAddressService.GetUserId();
            string username = _ipAddressService.GetUserName();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);

            var ApprovalReq = _imapper.Map<ApprovalRequest>(request);
           var isPending = await _approvalRequestQuery.IsAnyApprovalPending(ApprovalReq.Id,cancellationToken);
            if (isPending)
            {
                ApprovalReq.StatusId = statusPending.Id;
            }
            else
            {
                ApprovalReq.StatusId = request.IsApproved == 1 ? statusApproved.Id : statusRejected.Id;
            }
            
            ApprovalReq.ModifiedIP = currentIp;
            ApprovalReq.ModifiedDate = currentTime;
            ApprovalReq.ModifiedBy = userId;
            ApprovalReq.ModifiedByName = username;

            foreach (var approval in ApprovalReq.ApprovalRequestLines)
            {
                approval.StatusId = request.IsApproved == 1 ? statusApproved.Id : statusRejected.Id;
            }
            
                var result = await _approvalRequestCommand.Approve(ApprovalReq,cancellationToken);
                


            
                var ApprovalReqLine = _imapper.Map<List<UpdateApprovedQtyDto>>(request.ApprovalRequestLine);
                
                var correlationId = Guid.NewGuid();
                var @event = new ApprovedRejectedEvent
                {
                    CorrelationId = correlationId,
                    IndentId = request.ModuleTransactionId,
                    ApprovedQty = ApprovalReqLine
                };

                await _eventPublisher.SaveEventAsync(@event);
                await _eventPublisher.PublishPendingEventsAsync();
        


                return true;          
        }
    }
}