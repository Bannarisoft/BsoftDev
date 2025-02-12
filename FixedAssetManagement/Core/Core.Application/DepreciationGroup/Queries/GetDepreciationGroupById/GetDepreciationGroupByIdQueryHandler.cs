
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepreciationGroup.Queries.GetDepreciationGroupById
{
    public class GetDepreciationGroupByIdQueryHandler : IRequestHandler<GetDepreciationGroupByIdQuery, ApiResponseDTO<DepreciationGroupDTO>>
    {
        private readonly IDepreciationGroupQueryRepository _depreciationGroupRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetDepreciationGroupByIdQueryHandler(IDepreciationGroupQueryRepository depreciationGroupRepository,  IMapper mapper, IMediator mediator)
        {
            _depreciationGroupRepository =depreciationGroupRepository;
            _mapper =mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<DepreciationGroupDTO>> Handle(GetDepreciationGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var depreciationGroup = await _depreciationGroupRepository.GetByIdAsync(request.Id);                
            var depreciationGroupDto = _mapper.Map<DepreciationGroupDTO>(depreciationGroup);
            if (depreciationGroup is null)
            {                
                return new ApiResponseDTO<DepreciationGroupDTO>
                {
                    IsSuccess = false,
                    Message = "DepreciationGroup with ID {request.Id} not found."
                };   
            }       
                //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: depreciationGroupDto.Code ?? string.Empty,        
                actionName: depreciationGroupDto.DepreciationGroupName ?? string.Empty,                
                details: $"DepreciationGroup '{depreciationGroupDto.DepreciationGroupName}' was created. Code: {depreciationGroupDto.Code}",
                module:"DepreciationGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<DepreciationGroupDTO>
            {
                IsSuccess = true,
                Message = "Success",
                Data = depreciationGroupDto
            };       
        }
    }
}