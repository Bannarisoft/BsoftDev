using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetDetailsById
{
    public class GetAsstDetailsByIdQuery : IRequest<ApiResponseDTO<AssetDetailsResponse>>
    {
       public int AssetId { get; set; }     
    }
}