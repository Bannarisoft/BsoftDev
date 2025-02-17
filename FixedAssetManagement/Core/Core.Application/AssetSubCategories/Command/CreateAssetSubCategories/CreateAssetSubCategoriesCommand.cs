using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetSubCategories.Command.CreateAssetSubCategories
{
    public class CreateAssetSubCategoriesCommand :IRequest<ApiResponseDTO<int>> 
    {
        public string? Code { get; set; }
        public string? SubCategoryName { get; set; }
        public string? Description { get; set; }
        public int AssetCategoriesId { get; set; }
    }
}