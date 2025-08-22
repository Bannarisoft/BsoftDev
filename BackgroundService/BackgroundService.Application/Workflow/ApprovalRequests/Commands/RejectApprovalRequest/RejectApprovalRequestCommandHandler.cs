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
using MediatR;


namespace BackgroundService.Application.Workflow.ApprovalRequests.Commands.RejectApprovalRequest
{
    public class RejectApprovalRequestCommandHandler : IRequestHandler<RejectApprovalRequestCommand, bool>
    {
         private readonly IMiscMasterQueryRepository _miscMasterQuery;
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IApprovalRequestCommand _approvalRequestCommand;
        private readonly IMapper _imapper;
        public RejectApprovalRequestCommandHandler(IMiscMasterQueryRepository miscMasterQuery, IIPAddressService ipAddressService, ITimeZoneService timeZoneService,
         IApprovalRequestCommand approvalRequestCommand, IMapper imapper)
        {
            _miscMasterQuery = miscMasterQuery;
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;
            _approvalRequestCommand = approvalRequestCommand;
            _imapper = imapper;
        }

        public async Task<bool> Handle(RejectApprovalRequestCommand request, CancellationToken cancellationToken)
        {
             var status = await _miscMasterQuery.GetMiscMasterByName(MiscEnumEntity.ApprovalStatus, MiscEnumEntity.Rejected);
            string currentIp = _ipAddressService.GetSystemIPAddress();
            int userId = _ipAddressService.GetUserId();
            string username = _ipAddressService.GetUserName();
            var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
            var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);

             var ApprovalReq = _imapper.Map<ApprovalRequest>(request);

                 ApprovalReq.StatusId = status.Id;
               ApprovalReq.ModifiedIP = currentIp;
               ApprovalReq.ModifiedDate = currentTime;
               ApprovalReq.ModifiedBy = userId;
               ApprovalReq.ModifiedByName = username;

            // var ApprovalReq = new ApprovalRequest
            // {
            //     Id = request.Id,
            //     StatusId = status.Id,
            //     ModifiedIP = currentIp,
            //     ModifiedDate = currentTime,
            //     ModifiedBy = userId,
            //     ModifiedByName = username,
            //     Action = "test"
            // };
            var result = await _approvalRequestCommand.Reject(ApprovalReq);

            return result;   
        }
    }
}