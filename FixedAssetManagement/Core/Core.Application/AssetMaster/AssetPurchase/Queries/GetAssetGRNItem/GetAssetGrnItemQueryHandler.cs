using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGRNItem
{
    public class GetAssetGrnItemQueryHandler : IRequestHandler<GetAssetGrnItemQuery, ApiResponseDTO<List<AssetGrnItemDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IAssetPurchaseQueryRepository _iAssetPurchaseQueryRepository;

        public GetAssetGrnItemQueryHandler(IMapper mapper, IMediator mediator, IAssetPurchaseQueryRepository iAssetPurchaseQueryRepository)
        {
            _mapper = mapper;
            _mediator = mediator;
            _iAssetPurchaseQueryRepository = iAssetPurchaseQueryRepository;
        }

        public async Task<ApiResponseDTO<List<AssetGrnItemDto>>> Handle(GetAssetGrnItemQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetPurchaseQueryRepository.GetAssetGrnItem(request.OldUnitId, request.GrnNo);
            var assetunits  = _mapper.Map<List<AssetGrnItemDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetGrnItem",
                    actionCode: "GetAll",        
                    actionName: "GrnItem",
                    details: $"GrnItem details was fetched.",
                    module:"GrnItem"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetGrnItemDto>> { IsSuccess = true, Message = "Success", Data = assetunits };
        }
    }
}