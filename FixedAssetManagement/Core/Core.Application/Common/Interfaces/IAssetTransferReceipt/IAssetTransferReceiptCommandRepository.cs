using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetTransferReceipt
{
    public interface IAssetTransferReceiptCommandRepository
    {
        Task<int> CreateAsync(AssetTransferReceiptHdr assetTransferReceiptHdr);
    }
}