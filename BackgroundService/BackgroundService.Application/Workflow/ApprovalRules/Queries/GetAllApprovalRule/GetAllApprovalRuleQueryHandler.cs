using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.HttpResponse;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRule;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRules.Queries.GetAllApprovalRule
{
    public class GetAllApprovalRuleQueryHandler : IRequestHandler<GetAllApprovalRuleQuery, ApiResponseDTO<List<ApprovalRuleDto>>>
    {
         private readonly IApprovalRuleQuery _approvalRuleQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetAllApprovalRuleQueryHandler(IApprovalRuleQuery approvalRuleQuery, IMediator mediator, IMapper mapper)
        {
            _approvalRuleQuery = approvalRuleQuery;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<ApprovalRuleDto>>> Handle(GetAllApprovalRuleQuery request, CancellationToken cancellationToken)
        {
            var (ApprovalRule, TotalCount) = await _approvalRuleQuery.GetAllApprovalRuleAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var ApprovalRuleDto = _mapper.Map<List<ApprovalRuleDto>>(ApprovalRule);


            return new ApiResponseDTO<List<ApprovalRuleDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = ApprovalRuleDto,
                TotalCount = TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}