using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetSubCategories.Command.UpdateAssetSubCategories
{
    public class UpdateAssetSubCategoriesCommand:IRequest<ApiResponseDTO<int>> 
    {
        public int Id { get; set; }
        public string? SubCategoryName { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public int AssetCategoriesId { get; set; }
        public byte IsActive { get; set; }
    }
}