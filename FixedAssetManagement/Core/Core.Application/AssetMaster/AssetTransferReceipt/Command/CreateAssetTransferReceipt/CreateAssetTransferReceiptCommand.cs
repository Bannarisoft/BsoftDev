using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Command.CreateAssetTransferReceipt
{
    public class CreateAssetTransferReceiptCommand   : IRequest<ApiResponseDTO<int>>
    {
          public AssetTransferReceiptHdrDto? AssetTransferReceiptHdrDto { get; set; }
    }
}