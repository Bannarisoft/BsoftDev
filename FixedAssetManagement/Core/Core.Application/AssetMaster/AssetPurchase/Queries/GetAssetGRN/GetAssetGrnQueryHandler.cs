using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGRN
{
    public class GetAssetGrnQueryHandler :  IRequestHandler<GetAssetGrnQuery,ApiResponseDTO<List<GetAssetGrnDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IAssetPurchaseQueryRepository _iAssetPurchaseQueryRepository;

        public GetAssetGrnQueryHandler(IMapper mapper, IMediator mediator, IAssetPurchaseQueryRepository iAssetPurchaseQueryRepository)
        {
            _mapper = mapper;
            _mediator = mediator;
            _iAssetPurchaseQueryRepository = iAssetPurchaseQueryRepository;
        }

        public async Task<ApiResponseDTO<List<GetAssetGrnDto>>> Handle(GetAssetGrnQuery request, CancellationToken cancellationToken)
        {
             var result = await _iAssetPurchaseQueryRepository.GetAssetGrnNo(request.OldUnitId,request.AssetSourceId,request.SearchGrnNo);
            var assetunits  = _mapper.Map<List<GetAssetGrnDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "GetAll",        
                    actionName: "GRNNO",
                    details: $"GRN details was fetched.",
                    module:"GRNNO"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<GetAssetGrnDto>> { IsSuccess = true, Message = "Success", Data = assetunits };
        }
    }
}