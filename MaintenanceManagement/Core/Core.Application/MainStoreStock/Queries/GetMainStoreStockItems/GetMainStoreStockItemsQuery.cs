using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MainStoreStock.Queries.GetMainStoreStockItems
{
    public class GetMainStoreStockItemsQuery : IRequest<ApiResponseDTO<List<MainStoresStockItemsDto>>>
    {
        
        public string? OldUnitcode { get; set; }
        public string? GroupCode { get; set; }
    }
}