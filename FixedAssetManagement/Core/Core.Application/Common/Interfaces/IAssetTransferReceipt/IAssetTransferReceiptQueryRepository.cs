using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptDetails;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptDetailsById;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetRecieptDtlPending;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetTransferReceipt
{
    public interface IAssetTransferReceiptQueryRepository
    {
        Task<(List<AssetTransferReceiptPendingDto>, int)> GetAllPendingAssetTransferAsync(
        int PageNumber, 
        int PageSize, 
        int? AssetTransferId,
        string? SearchTerm, 
        DateTimeOffset? FromDate, 
        DateTimeOffset? ToDate);
        
        //Task<List<AssetTrasnferReceiptHdrPendingDto>> GetAllPendingAssetTransferDtlAsync(int assetTransferId);

         Task<AssetTrasnferReceiptHdrPendingDto?> GetAssetTransferByIdAsync(int assetTransferId);

        Task<(List<AssetReceiptDetailsDto>, int)> GetAllAssetReceiptDetails(
        int PageNumber, 
        int PageSize, 
        string? SearchTerm, 
        DateTimeOffset? FromDate, 
        DateTimeOffset? ToDate);

        Task<List<AssetReceiptDetailsByIdDto>> GetByAssetReceiptId(int AssetReceiptId);
     
        Task<AssetTransferDto?> GetByAssetTransferId(int assetTransferId);
        



    }
}