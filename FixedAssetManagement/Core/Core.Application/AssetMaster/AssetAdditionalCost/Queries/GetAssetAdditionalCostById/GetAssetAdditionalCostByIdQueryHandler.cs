using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCost;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAdditionalCost;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCostById
{
    public class GetAssetAdditionalCostByIdQueryHandler : IRequestHandler<GetAssetAdditionalCostByIdQuery,ApiResponseDTO<AssetAdditionalCostDto>>
    {
        private readonly IAssetAdditionalCostQueryRepository _iAssetAdditionalCostQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetAdditionalCostByIdQueryHandler(IAssetAdditionalCostQueryRepository iAssetAdditionalCostQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetAdditionalCostQueryRepository = iAssetAdditionalCostQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<AssetAdditionalCostDto>> Handle(GetAssetAdditionalCostByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetAdditionalCostQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<AssetAdditionalCostDto> { IsSuccess = false, Message =$"AssetAdditionalCost ID {request.Id} not found." };
            }
            // Map a single entity
            var assetGroup = _mapper.Map<AssetAdditionalCostDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "AssetAdditionalCost",        
                    actionName: "AssetAdditionalCost",
                    details: $"AssetAdditionalCost details {assetGroup.Id} was fetched.",
                    module:"AssetAdditionalCost"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<AssetAdditionalCostDto> { IsSuccess = true, Message = "Success", Data = assetGroup };
        }
    }
}