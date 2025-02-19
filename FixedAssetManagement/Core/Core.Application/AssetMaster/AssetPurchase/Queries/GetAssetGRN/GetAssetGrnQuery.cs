using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGRN
{
    public class GetAssetGrnQuery : IRequest<ApiResponseDTO<List<GetAssetGrnDto>>>
    {
         public int OldUnitId { get; set; } 
    }
}