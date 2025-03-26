using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory
{
    public class GetAssetsByCategoryQuery  : IRequest<List<GetAssetMasterDto>>
    {
         public int AssetCategoryId { get; set; }
    }
}