using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetTransferReceipt
{
    public interface IAssetTransferReceiptQueryRepository
    {
        Task<(List<AssetTransferReceiptPendingDto>, int)> GetAllPendingAssetTransferAsync(
        int PageNumber, 
        int PageSize, 
        string? SearchTerm, 
        DateTimeOffset? FromDate, 
        DateTimeOffset? ToDate);
    }
}