using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetCategories.Queries.GetAssetCategoriesByAssetGroupId
{
    public class GetAssetCategoriesByAssetGroupIdQuery : IRequest<ApiResponseDTO<List<AssetCategoriesAutoCompleteDto>>> 
    {
        public int AssetGroupId { get; set; }
    }
}