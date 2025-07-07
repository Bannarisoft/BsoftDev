using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MainStoreStock.Queries.GetItemStockbyId
{
    public class GetItemStockbyIdQuery : IRequest<MainStoreItemStockDto>
    {
        public string? OldUnitcode { get; set; }
        public string? ItemCode { get; set; }
    }
}