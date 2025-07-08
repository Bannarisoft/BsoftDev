using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MainStoreStock.Queries.GetMainStoreStock
{
    public class GetMainStoreStockQuery : IRequest<List<MainStoresStockDto>>
    {
        public string? OldUnitcode { get; set; }
        public string? GroupCode { get; set; }
    }
}