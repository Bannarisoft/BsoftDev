using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCost;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Mappings.AssetMaster;
using MediatR;

namespace Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCostById
{
    public class GetAssetAdditionalCostByIdQuery : IRequest<ApiResponseDTO<AssetAdditionalCostDto>>
    {
        public int Id { get; set; }
    }
}