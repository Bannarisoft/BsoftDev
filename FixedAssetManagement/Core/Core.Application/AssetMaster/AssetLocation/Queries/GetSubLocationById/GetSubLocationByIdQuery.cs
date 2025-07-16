using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetLocation.Queries.GetSubLocationById
{
    public class GetSubLocationByIdQuery : IRequest<ApiResponseDTO<List<GetAssetSubLocationDto>>>
    {
         public int Id { get; set; }
    }
}