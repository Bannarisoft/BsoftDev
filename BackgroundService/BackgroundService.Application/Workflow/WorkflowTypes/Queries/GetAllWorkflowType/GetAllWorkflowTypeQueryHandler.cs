using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.HttpResponse;
using BackgroundService.Application.Workflow.Common.Interfaces.IWorkflowType;
using MediatR;

namespace BackgroundService.Application.Workflow.WorkflowTypes.Queries.GetAllWorkflowType
{
    public class GetAllWorkflowTypeQueryHandler : IRequestHandler<GetAllWorkflowTypeQuery, ApiResponseDTO<List<WorkflowTypeDto>>>
    {
        private readonly IWorkflowTypeQuery _workflowTypeQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetAllWorkflowTypeQueryHandler(IWorkflowTypeQuery workflowTypeQuery, IMediator mediator, IMapper mapper)
        {
            _workflowTypeQuery = workflowTypeQuery;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<WorkflowTypeDto>>> Handle(GetAllWorkflowTypeQuery request, CancellationToken cancellationToken)
        {
            var (WorkflowType, TotalCount) = await _workflowTypeQuery.GetAllWorkflowTypeAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var WorkflowTypeDto = _mapper.Map<List<WorkflowTypeDto>>(WorkflowType);


            return new ApiResponseDTO<List<WorkflowTypeDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = WorkflowTypeDto,
                TotalCount = TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}