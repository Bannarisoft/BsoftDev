using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Domain.Entities.AssetPurchase;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGrnDetails
{
    public class GetAssetDetailsQuery : IRequest<ApiResponseDTO<List<AssetGrnDetails>>>
    {
        public string?  OldUnitId { get; set; } 
        public int AssetSourceId { get; set; }
        public int GrnNo { get; set; } 
        public int GrnSerialNo { get; set; }
    }
}