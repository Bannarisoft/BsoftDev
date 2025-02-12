using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepreciationGroup.Queries.GetDepreciationGroup
{
    public class GetDepreciationGroupQueryHandler : IRequestHandler<GetDepreciationGroupQuery, ApiResponseDTO<List<DepreciationGroupDTO>>>
    {
        private readonly IDepreciationGroupQueryRepository _depreciationGroupRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetDepreciationGroupQueryHandler(IDepreciationGroupQueryRepository depreciationGroupRepository , IMapper mapper, IMediator mediator)
        {
            _depreciationGroupRepository = depreciationGroupRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<DepreciationGroupDTO>>> Handle(GetDepreciationGroupQuery request, CancellationToken cancellationToken)
        {
            var (depreciationGroup, totalCount) = await _depreciationGroupRepository.GetAllDepreciationGroupAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var depreciationGroupList = _mapper.Map<List<DepreciationGroupDTO>>(depreciationGroup);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"DepreciationGroup details was fetched.",
                module:"DepreciationGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<DepreciationGroupDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = depreciationGroupList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };            
        }
    }
}