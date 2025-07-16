using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetSourceAutoComplete
{
    public class GetAssetUnitAutoCompleteQuery : IRequest<ApiResponseDTO<List<AssetUnitAutoCompleteDto>>>
    {
        public string? Username { get; set; }
    }
}