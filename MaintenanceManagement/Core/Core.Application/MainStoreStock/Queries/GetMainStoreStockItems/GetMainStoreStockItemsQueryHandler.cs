using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMainStoreStock;
using MediatR;

namespace Core.Application.MainStoreStock.Queries.GetMainStoreStockItems
{
    public class GetMainStoreStockItemsQueryHandler : IRequestHandler<GetMainStoreStockItemsQuery,ApiResponseDTO<List<MainStoresStockItemsDto>>>
    {
        private readonly IMainStoreStockQueryRepository _mainStoreStockQueryRepository;  
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetMainStoreStockItemsQueryHandler(IMainStoreStockQueryRepository mainStoreStockQueryRepository, IMapper mapper, IMediator mediator)
        {
            _mainStoreStockQueryRepository = mainStoreStockQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<MainStoresStockItemsDto>>> Handle(GetMainStoreStockItemsQuery request, CancellationToken cancellationToken)
        {
            var result = await _mainStoreStockQueryRepository.GetStockItemsCodes(request.OldUnitcode,request.GroupCode);
            var substores = _mapper.Map<List<MainStoresStockItemsDto>>(result);
             //Domain Event
                var domainEvent = new Domain.Events.AuditLogsDomainEvent(
                    actionDetail: "GetAllStock",
                    actionCode: "GetMainStoreStockItemsCodesQuery",        
                    actionName: substores.Count.ToString(),
                    details: $"Stock ItemCodes details was fetched.",
                    module:"MainStoresStockItemCodes"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<MainStoresStockItemsDto>> { IsSuccess = true, Message = "Success", Data = substores };
        }
    }
}