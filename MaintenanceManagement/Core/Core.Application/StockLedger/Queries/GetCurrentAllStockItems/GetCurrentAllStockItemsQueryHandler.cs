using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IStcokLedger;
using Core.Application.StockLedger.Queries.GetCurrentStock;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.StockLedger.Queries.GetCurrentAllStockItems
{
    public class GetCurrentAllStockItemsQueryHandler : IRequestHandler<GetCurrentAllStockItemsQuery,ApiResponseDTO<List<CurrentStockDto>>>
    {
        private readonly IStockLedgerQueryRepository _stockLedgerQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetCurrentAllStockItemsQueryHandler(IStockLedgerQueryRepository stockLedgerQueryRepository, IMapper mapper, IMediator mediator)
        {
            _stockLedgerQueryRepository = stockLedgerQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<CurrentStockDto>>> Handle(GetCurrentAllStockItemsQuery request, CancellationToken cancellationToken)
        {
            var result = await _stockLedgerQueryRepository.GetStockDetails(request.OldUnitcode);
            var substores = _mapper.Map<List<CurrentStockDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAllStock",
                    actionCode: "GetCurrentAllStockItemsQuery",        
                    actionName: substores.Count.ToString(),
                    details: $"Stock details was fetched.",
                    module:"SubStoresStock"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<CurrentStockDto>> { IsSuccess = true, Message = "Success", Data = substores };
        }
    }
}