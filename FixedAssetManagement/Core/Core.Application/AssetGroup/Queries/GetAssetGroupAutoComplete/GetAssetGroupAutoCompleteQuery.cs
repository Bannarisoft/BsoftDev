using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetGroup.Queries.GetAssetGroup;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetGroup.Queries.GetAssetGroupAutoComplete
{
    public class GetAssetGroupAutoCompleteQuery : IRequest<ApiResponseDTO<List<AssetGroupAutoCompleteDTO>>>
    {
        public string? SearchPattern { get; set; }
       // public int AssetGroupId { get; set; }
    }
}