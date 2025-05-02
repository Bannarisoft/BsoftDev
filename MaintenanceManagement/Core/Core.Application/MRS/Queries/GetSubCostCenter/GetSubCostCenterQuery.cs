using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MRS.Queries.GetSubCostCenter
{
    public class GetSubCostCenterQuery : IRequest<ApiResponseDTO<List<MSubCostCenterDto>>>
    {
         public string? OldUnitcode { get; set; }
    }
}