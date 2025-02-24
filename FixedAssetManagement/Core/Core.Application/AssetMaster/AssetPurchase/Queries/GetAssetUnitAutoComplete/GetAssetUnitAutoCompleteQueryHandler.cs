using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetCategories.Queries.GetAssetCategoriesAutoComplete;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetSourceAutoComplete;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries
{
    public class GetAssetUnitAutoCompleteQueryHandler  :  IRequestHandler<GetAssetUnitAutoCompleteQuery,ApiResponseDTO<List<AssetUnitAutoCompleteDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
         private readonly IAssetPurchaseQueryRepository _iAssetPurchaseQueryRepository;


        public GetAssetUnitAutoCompleteQueryHandler(IMapper mapper, IMediator mediator, IAssetPurchaseQueryRepository iAssetPurchaseQueryRepository)
        {
            _mapper = mapper;
            _mediator = mediator;
            _iAssetPurchaseQueryRepository=iAssetPurchaseQueryRepository;
        }


        public async Task<ApiResponseDTO<List<AssetUnitAutoCompleteDto>>> Handle(GetAssetUnitAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetPurchaseQueryRepository.GetAssetUnit(request.Username);
            var assetunits  = _mapper.Map<List<AssetUnitAutoCompleteDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "GetAll",        
                    actionName: "Assetunit",
                    details: $"Assetunit details was fetched.",
                    module:"Assetunit"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetUnitAutoCompleteDto>> { IsSuccess = true, Message = "Success", Data = assetunits };
        }
    }
}