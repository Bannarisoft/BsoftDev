using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetCategories.Queries.GetAssetCategoriesAutoComplete
{
    public class GetAssetCategoriesAutoCompleteQuery : IRequest<ApiResponseDTO<List<AssetCategoriesAutoCompleteDto>>>
    {
        public string? SearchPattern { get; set; }
    }
}