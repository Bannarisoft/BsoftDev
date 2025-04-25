using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.StockLedger.Queries.GetCurrentStock
{
    public class GetCurrentStockQuery :  IRequest<ApiResponseDTO<CurrentStockDto>>
    {
        public string? OldUnitId { get; set; }
        public string? ItemCode { get; set; }

    }
}