using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MRS.Queries.GetPendingQty
{
    public class GetPendingQtyQuery : IRequest<ApiResponseDTO<GetPendingQtyDto>>
    {
        public string? OldUnitcode { get; set; } 
        public string? ItemCode { get; set; }
    }
}