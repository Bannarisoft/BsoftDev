using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTranferedById
{
    public class GetAssetTranferedByIdQuery  : IRequest<ApiResponseDTO<AssetTransferJsonDto>>
    {       
        public int AssetTransferId { get; set; }

            
    }
}