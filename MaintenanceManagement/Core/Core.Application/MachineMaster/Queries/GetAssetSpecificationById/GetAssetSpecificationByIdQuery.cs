using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetAssetSpecificationById
{
    public class GetAssetSpecificationByIdQuery : IRequest<ApiResponseDTO<List<AssetSpecificationByAssetIdDto>>>
    {
        public int AssetId { get; set; }
    }
}