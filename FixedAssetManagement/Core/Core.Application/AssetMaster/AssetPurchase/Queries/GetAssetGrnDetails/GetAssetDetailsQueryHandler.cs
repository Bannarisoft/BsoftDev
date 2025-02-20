using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase;
using Core.Domain.Entities.AssetPurchase;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGrnDetails
{
    public class GetAssetDetailsQueryHandler : IRequestHandler<GetAssetDetailsQuery, ApiResponseDTO<List<AssetGrnDetails>>>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IAssetPurchaseQueryRepository _iAssetPurchaseQueryRepository;

        public GetAssetDetailsQueryHandler(IMapper mapper, IMediator mediator, IAssetPurchaseQueryRepository iAssetPurchaseQueryRepository)
        {
            _mapper = mapper;
            _mediator = mediator;
            _iAssetPurchaseQueryRepository = iAssetPurchaseQueryRepository;
        }

        public async Task<ApiResponseDTO<List<AssetGrnDetails>>> Handle(GetAssetDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = await _iAssetPurchaseQueryRepository.GetAssetGrnItemDetails(request.OldUnitId, request.GrnNo,request.GrnSerialNo);
            var assetunits  = _mapper.Map<List<AssetGrnDetails>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetGrnDetails",
                    actionCode: "GetAll",        
                    actionName: "GrnItemDetails",
                    details: $"GrnItemDetails details was fetched.",
                    module:"GrnItemDetails"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetGrnDetails>> { IsSuccess = true, Message = "Success", Data = assetunits };
        }
    }
}