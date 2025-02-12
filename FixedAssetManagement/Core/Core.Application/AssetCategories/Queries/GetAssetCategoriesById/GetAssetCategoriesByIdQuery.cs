using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetCategories.Queries.GetAssetCategories;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetCategories.Queries.GetAssetCategoriesById
{
    public class GetAssetCategoriesByIdQuery : IRequest<ApiResponseDTO<AssetCategoriesDto>>
    {
        public int Id { get; set; }
    }
}