using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.HttpResponse;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using Contracts.Interfaces.External.IUser;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Queries.GetAllApprovalRequest
{
    public class GetAllApprovalRequestQueryHandler : IRequestHandler<GetAllApprovalRequestQuery, ApiResponseDTO<List<ApprovalRequestDto>>>
    {
         private readonly IApprovalRequestQuery _approvalRequestQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUsersAllGrpcClient _usersAllGrpcClient;
        public GetAllApprovalRequestQueryHandler(IApprovalRequestQuery approvalRequestQuery, IMediator mediator, IMapper mapper,
         IUsersAllGrpcClient usersAllGrpcClient)
        {
            _approvalRequestQuery = approvalRequestQuery;
            _mediator = mediator;
            _mapper = mapper;
            _usersAllGrpcClient = usersAllGrpcClient;
        }
        public async Task<ApiResponseDTO<List<ApprovalRequestDto>>> Handle(GetAllApprovalRequestQuery request, CancellationToken cancellationToken)
        {
            var (ApprovalReq, TotalCount) = await _approvalRequestQuery.GetAllApprovalRequestAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var ApprovalReqDto = _mapper.Map<List<ApprovalRequestDto>>(ApprovalReq);

            var users = await _usersAllGrpcClient.GetUserAllAsync();
            var userDict = users.ToDictionary(u => u.UserId, u => u.UserName);

            foreach (var approval in ApprovalReqDto)
            {
                if (userDict.TryGetValue(approval.TargetTypeId, out var approverName))
                {
                    approval.ApproverName = approverName;
                }
                else
                {
                    approval.ApproverName = "Unknown User";
                }
            }
            return new ApiResponseDTO<List<ApprovalRequestDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = ApprovalReqDto,
                TotalCount = TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}