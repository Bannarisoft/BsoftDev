using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRule;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRules.Queries.GetApprovalRuleAutoComplete
{
    public class GetApprovalRuleAutoCompleteQueryHandler : IRequestHandler<GetApprovalRuleAutoCompleteQuery, List<ApprovalRuleAutoCompleteDto>>
    {
        private readonly IApprovalRuleQuery _approvalRuleQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetApprovalRuleAutoCompleteQueryHandler(IApprovalRuleQuery approvalRuleQuery, IMediator mediator, IMapper mapper)
        {
            _approvalRuleQuery = approvalRuleQuery;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<List<ApprovalRuleAutoCompleteDto>> Handle(GetApprovalRuleAutoCompleteQuery request, CancellationToken cancellationToken)
        {
             var Result = await _approvalRuleQuery.GetApprovalRuleAutoComplete(request.SearchPattern ?? string.Empty);
            var ApprovalRule = _mapper.Map<List<ApprovalRuleAutoCompleteDto>>(Result);
            
            return ApprovalRule;
        }
    }
}