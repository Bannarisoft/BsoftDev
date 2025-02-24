using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchaseById
{
    public class GetAssetPurchaseByIdQueryHandler : IRequestHandler<GetAssetPurchaseByIdQuery,ApiResponseDTO<AssetPurchaseDetailsDto>>
    {
         private readonly IAssetPurchaseQueryRepository _iAssetPurchaseQueryRepository;  
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetAssetPurchaseByIdQueryHandler(IAssetPurchaseQueryRepository iAssetPurchaseQueryRepository, IMapper mapper, IMediator mediator)
        {
            _iAssetPurchaseQueryRepository = iAssetPurchaseQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<AssetPurchaseDetailsDto>> Handle(GetAssetPurchaseByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetPurchaseQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<AssetPurchaseDetailsDto> { IsSuccess = false, Message =$"AssetPurchase ID {request.Id} not found." };
            }
            // Map a single entity
            var assetGroup = _mapper.Map<AssetPurchaseDetailsDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"Asset Purchase details {request.Id} was fetched.",
                    module:"AssetPurchaseDetails"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<AssetPurchaseDetailsDto> { IsSuccess = true, Message = "Success", Data = assetGroup };
        }
    }
}