using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetSourceAutoComplete
{
    public class GetAssetSourceAutoCompleteQuery  : IRequest<ApiResponseDTO<List<AssetSourceAutoCompleteDto>>>
    {
        public string? SearchPattern { get; set; }
    }
}