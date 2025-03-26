using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategories;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetSubCategories.Queries.GetAssetSubCategoriesByCategoryId
{
    public class GetAssetSubCategoriesByCategoryIdQuery : IRequest<ApiResponseDTO<List<AssetSubCategoriesAutoCompleteDto>>> 
    {
        public int AssetCategoriesId { get; set; }
    }
        
    
}