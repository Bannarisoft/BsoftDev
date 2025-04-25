using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.StockLedger.Queries.GetCurrentStockItemsById
{
    public class GetCurrentStockItemsByIdQuery : IRequest<ApiResponseDTO<List<StockItemCodeDto>>>
    {
        public string? OldUnitcode { get; set; }
    }
}