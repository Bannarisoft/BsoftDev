using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmc;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAmc;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmcById
{
    public class GetAssetAmcByIdQueryHandler : IRequestHandler<GetAssetAmcByIdQuery, ApiResponseDTO<AssetAmcDto>>
    {
        private readonly IAssetAmcQueryRepository _iAssetAmcQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetAssetAmcByIdQueryHandler(IAssetAmcQueryRepository iAssetAmcQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetAmcQueryRepository = iAssetAmcQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<AssetAmcDto>> Handle(GetAssetAmcByIdQuery request, CancellationToken cancellationToken)
        {
           var result = await _iAssetAmcQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<AssetAmcDto> { IsSuccess = false, Message =$"Asset ID {request.Id} not found." }; 
            }
            // Map a single entity
            var assetamc = _mapper.Map<AssetAmcDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "AssetAmc",        
                    actionName: "GetById",
                    details: $"AssetAmc details {assetamc.Id} was fetched.",
                    module:"AssetAmc"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return new ApiResponseDTO<AssetAmcDto> { IsSuccess = true, Message = "Success", Data = assetamc };
        }
    }
}