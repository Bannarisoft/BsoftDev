using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetCategories.Command.CreateAssetCategories
{
    public class CreateAssetCategoriesCommand :IRequest<ApiResponseDTO<int>> 
    {  
        public string? Code { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public int AssetGroupId { get; set; }

    }
}