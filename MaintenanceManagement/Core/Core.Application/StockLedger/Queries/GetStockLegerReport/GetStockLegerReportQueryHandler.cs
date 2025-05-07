using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IStcokLedger;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.StockLedger.Queries.GetStockLegerReport
{
    public class GetStockLegerReportQueryHandler : IRequestHandler<GetStockLegerReportQuery,ApiResponseDTO<List<StockLedgerReportDto>>>
    {
        private readonly IStockLedgerQueryRepository _stockLedgerQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        
        public GetStockLegerReportQueryHandler(IStockLedgerQueryRepository stockLedgerQueryRepository, IMapper mapper, IMediator mediator)
        {
            _stockLedgerQueryRepository = stockLedgerQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<StockLedgerReportDto>>> Handle(GetStockLegerReportQuery request, CancellationToken cancellationToken)
        {
            var result = await _stockLedgerQueryRepository.GetSubStoresStockLedger(request.OldUnitcode, request.FromDate, request.ToDate, request.ItemCode);
            var substores = _mapper.Map<List<StockLedgerReportDto>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "SubStoresStockLedgerReport",
                    actionCode: "GetStockLegerReportQuery",        
                    actionName: substores.Count.ToString(),
                    details: $"Stock details was fetched.",
                    module:"SubStoresStockLedgerReport"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<StockLedgerReportDto>> { IsSuccess = true, Message = "Success", Data = substores };
        }
    }
}