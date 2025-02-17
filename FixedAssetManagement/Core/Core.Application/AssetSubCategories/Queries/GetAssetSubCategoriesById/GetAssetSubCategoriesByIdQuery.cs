using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategories;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetSubCategories.Queries.GetAssetSubCategoriesById
{
    public class GetAssetSubCategoriesByIdQuery: IRequest<ApiResponseDTO<AssetSubCategoriesDto>>
    {
        public int Id { get; set; }
    }
}