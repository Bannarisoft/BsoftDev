using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.StockLedger.Queries.GetCurrentStockItemsById;
using MediatR;

namespace Core.Application.StockLedger.Queries.GetCurrentAllItemsById
{
    public class GetCurrentAllItemsByIdQuery : IRequest<ApiResponseDTO<List<StockItemCodeDto>>>
    {
        public string? OldUnitcode { get; set; }
        public int DepartmentId { get; set; }
    }
}