using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.StockLedger.Queries.GetCurrentStock;
using MediatR;

namespace Core.Application.Reports.GetCurrentAllStockItems
{
    public class GetCurrentAllStockItemsQuery : IRequest<ApiResponseDTO<List<CurrentStockDto>>>
    {
        public string? OldUnitcode { get; set; }
    }
}