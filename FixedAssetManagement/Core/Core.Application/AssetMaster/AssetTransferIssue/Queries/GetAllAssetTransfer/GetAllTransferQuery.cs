using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAllAssetTransfer
{
    public class GetAllTransferQuery : IRequest<ApiResponseDTO<List<GetAllTransferDtlDto>>>
    
    {
       public int AssetTransferId  { get; set; }        
    }
}