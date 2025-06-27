using AutoMapper;
using Core.Application.AssetSubGroup.Queries.GetAssetSubGroup;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetSubGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetSubGroup.Queries.GetAssetSubGroupById
{
    public class GetAssetSubGroupByIdQueryHandler : IRequestHandler<GetAssetSubGroupByIdQuery,ApiResponseDTO<AssetSubGroupDto>>
    {
        private readonly IAssetSubGroupQueryRepository _iAssetSubGroupQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetSubGroupByIdQueryHandler(IAssetSubGroupQueryRepository iAssetSubGroupQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetSubGroupQueryRepository = iAssetSubGroupQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<AssetSubGroupDto>> Handle(GetAssetSubGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetSubGroupQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<AssetSubGroupDto> { IsSuccess = false, Message =$"AssetSubGroup ID {request.Id} not found." };
            }
            // Map a single entity
            var assetSubGroup = _mapper.Map<AssetSubGroupDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"AssetSubGroup details {assetSubGroup.Id} was fetched.",
                    module:"AssetSubGroup"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<AssetSubGroupDto> { IsSuccess = true, Message = "Success", Data = assetSubGroup };

        }
    }
}