using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptDetailsById;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetRecieptDtlPending
{
    public class GetAssetRecieptDtlPendingQuery : IRequest<ApiResponseDTO<List<AssetTransferReceiptDtlPendingDto>>>
    {
         public int AssetTransferId { get; set; }
    }
}