using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer
{
    public class GetAssetDetailsToTransferQuery : IRequest<ApiResponseDTO<GetAssetDetailsToTransferHdrDto>>
    {
         public int AssetId { get; set; }
    }
}