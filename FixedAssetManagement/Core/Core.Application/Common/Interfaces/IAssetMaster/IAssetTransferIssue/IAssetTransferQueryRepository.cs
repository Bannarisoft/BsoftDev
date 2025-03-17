using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAllAssetTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue
{
    public interface IAssetTransferQueryRepository 
    {
          Task<(List<AssetTransferDto>,int)> GetAllAsync(int PageNumber, int PageSize,string? SearchTerm, DateTimeOffset? FromDate = null, DateTimeOffset? ToDate = null);
           Task<List<GetAllTransferDtlDto>> GetAssetTransferByIDAsync(int assetTransferId);           
           Task<AssetTransferJsonDto?> GetAssetTransferByIdAsync(int assetTransferId);           
           Task<List<GetAssetMasterDto>> GetAssetsByCategoryAsync(int assetCategoryId);             
           Task<GetAssetDetailsToTransferHdrDto?> GetAssetDetailsToTransferByIdAsync(int assetId); 
           Task<bool> IsAssetPendingOrApprovedAsync(int assetId);

         
         

           
    }
}