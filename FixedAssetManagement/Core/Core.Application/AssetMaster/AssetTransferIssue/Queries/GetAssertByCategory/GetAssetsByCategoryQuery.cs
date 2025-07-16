using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory
{
    public class GetAssetsByCategoryQuery  : IRequest<ApiResponseDTO<List<GetAssetMasterDto>>>
    {
         public int AssetCategoryId { get; set; }
         public int AssetDepartmentId { get; set;}
    }
}