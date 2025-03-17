using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptDetailsById
{
    public class GetAssetReceiptDetailsByIdQuery : IRequest<ApiResponseDTO<List<AssetReceiptDetailsByIdDto>>>
    {
        public int AssetReceiptId { get; set; }
    }
}