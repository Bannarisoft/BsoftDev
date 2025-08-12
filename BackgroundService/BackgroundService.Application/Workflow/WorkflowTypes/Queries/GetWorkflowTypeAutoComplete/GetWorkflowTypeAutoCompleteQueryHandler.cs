using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Workflow.Common.Interfaces.IWorkflowType;
using MediatR;

namespace BackgroundService.Application.Workflow.WorkflowTypes.Queries.GetWorkflowTypeAutoComplete
{
    public class GetWorkflowTypeAutoCompleteQueryHandler : IRequestHandler<GetWorkflowTypeAutoCompleteQuery, List<GetWorkflowTypeAutoCompleteDto>>
    {
         private readonly IWorkflowTypeQuery _workflowTypeQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetWorkflowTypeAutoCompleteQueryHandler(IWorkflowTypeQuery workflowTypeQuery, IMediator mediator, IMapper mapper)
        {
            _workflowTypeQuery = workflowTypeQuery;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<List<GetWorkflowTypeAutoCompleteDto>> Handle(GetWorkflowTypeAutoCompleteQuery request, CancellationToken cancellationToken)
        {
             var Result = await _workflowTypeQuery.GetWorkflowTypeAutoComplete(request.SearchPattern ?? string.Empty);
            var WorkflowType = _mapper.Map<List<GetWorkflowTypeAutoCompleteDto>>(Result);
            
            return WorkflowType;
        }
    }
}