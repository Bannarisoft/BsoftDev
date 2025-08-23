using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Dto;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using Grpc.Core;
using GrpcServices.BackgroundService.Line;

namespace BackgroundService.API.GrpcServices
{
    public class ApproverListGrpcService : ApproverService.ApproverServiceBase
    {
        private readonly IApprovalRequestGrpcQuery _approvalRequestGrpcQuery;
        private readonly IMapper _mapper;
        public ApproverListGrpcService(IApprovalRequestGrpcQuery approvalRequestGrpcQuery, IMapper mapper)
        {
            _approvalRequestGrpcQuery = approvalRequestGrpcQuery;
            _mapper = mapper;
        }
        public override async Task<ApproverListResponse> GetApprover(ApproverRequest request, ServerCallContext context)
        {

            var data = await _approvalRequestGrpcQuery.GetApproverListByWorkFlowTypeAsync(request.ModuleTypeName);
             var ApprovalReqDto = _mapper.Map<List<ApprovalRequestLineDto>>(data);
            var response = new ApproverListResponse();
            
            foreach (var item in ApprovalReqDto)
              {
                  response.Approvalstatus.Add(new ApproverListDto
                  {
                      ApprovalRequestLineId = item.Id,
                      ModuleLineTransactionId = Convert.ToInt32(item.ModuleLineTransactionId),
                      Status = item.Status?.ToString() ?? string.Empty,
                      ApproverBinding = item.ApproverBinding?.ToString() ?? string.Empty,
                      ApproverValue = item.ApproverValue?.ToString() ?? string.Empty,
                      ApprovalRequestId = item.ApprovalRequestId
                  });
              }
            
              return response;

        }
    }
}