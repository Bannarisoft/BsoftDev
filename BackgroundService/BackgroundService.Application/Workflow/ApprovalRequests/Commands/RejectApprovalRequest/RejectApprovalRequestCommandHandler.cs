using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces.IMiscMaster;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Workflow;
using MediatR;


namespace BackgroundService.Application.Workflow.ApprovalRequests.Commands.RejectApprovalRequest
{
    public class RejectApprovalRequestCommandHandler : IRequestHandler<RejectApprovalRequestCommand, bool>
    {
         private readonly IMiscMasterQueryRepository _miscMasterQuery;
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IApprovalRequestCommand _approvalRequestCommand;
        public RejectApprovalRequestCommandHandler( IMiscMasterQueryRepository miscMasterQuery, IIPAddressService ipAddressService, ITimeZoneService timeZoneService,
         IApprovalRequestCommand approvalRequestCommand)
        {
            _miscMasterQuery = miscMasterQuery;
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;
            _approvalRequestCommand = approvalRequestCommand;
        }

        public async Task<bool> Handle(RejectApprovalRequestCommand request, CancellationToken cancellationToken)
        {
             var status = await _miscMasterQuery.GetMiscMasterByName(MiscEnumEntity.ApprovalStatus, MiscEnumEntity.Rejected);
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

            return result;   
        }
    }
}