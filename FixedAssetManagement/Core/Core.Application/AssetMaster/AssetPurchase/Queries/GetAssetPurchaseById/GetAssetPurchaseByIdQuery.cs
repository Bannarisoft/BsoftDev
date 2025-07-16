using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchaseById
{
    public class GetAssetPurchaseByIdQuery : IRequest<ApiResponseDTO<AssetPurchaseDetailsDto>>
    {
         public int Id { get; set; }
    }
}