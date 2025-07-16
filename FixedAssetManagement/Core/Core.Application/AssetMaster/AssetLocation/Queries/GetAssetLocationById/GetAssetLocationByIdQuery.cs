using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetLocation.Queries.GetAssetLocationById
{
    public class GetAssetLocationByIdQuery : IRequest<ApiResponseDTO<AssetLocationDto>>
    {
         public int Id { get; set; }
         
       
    }
}