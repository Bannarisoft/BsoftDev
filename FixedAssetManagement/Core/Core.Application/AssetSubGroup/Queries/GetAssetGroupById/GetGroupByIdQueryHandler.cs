using AutoMapper;
using Core.Application.AssetGroup.Queries.GetAssetGroupById;
using Core.Application.AssetSubGroup.Queries.GetAssetSubGroup;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetSubGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetSubGroup.Queries.GetAssetGroupById
{
    public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery,ApiResponseDTO<List<AssetSubGroupDto>>>
    {
        private readonly IAssetSubGroupQueryRepository _iAssetSubGroupQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetGroupByIdQueryHandler(IAssetSubGroupQueryRepository iAssetSubGroupQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetSubGroupQueryRepository = iAssetSubGroupQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<AssetSubGroupDto>>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetSubGroupQueryRepository.GetByGroupIdAsync(request.GroupId);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<List<AssetSubGroupDto>> { IsSuccess = false, Message = $"AssetSubGroup ID {request.GroupId} not found." };
            }
            // Map a single entity
            var assetSubGroup = _mapper.Map<List<AssetSubGroupDto>>(result);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "",
                actionName: "",
                details: $"AssetSubGroup details  was fetched.",
                module: "AssetSubGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            //return new ApiResponseDTO<AssetSubGroupDto> { IsSuccess = true, Message = "Success", Data = assetSubGroup };
           return new ApiResponseDTO<List<AssetSubGroupDto>> { IsSuccess = true, Message = "Success", Data = assetSubGroup };

        }
    }
}