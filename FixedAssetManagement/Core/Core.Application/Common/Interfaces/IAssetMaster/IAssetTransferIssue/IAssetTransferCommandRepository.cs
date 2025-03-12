using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue
{
    public interface IAssetTransferCommandRepository
    {
        Task<int> CreateAssetTransferAsync(AssetTransferIssueHdr assetTransferIssueHdr);
        Task<AssetTransferIssueDtl> CreateAssetTransferIssueAsync(AssetTransferIssueDtl assetTransferIssueDtl);

        Task<bool> UpdateAssetTransferAsync(AssetTransferIssueHdr assetTransferIssueHdr);
     //   Task<AssetTransferIssueDtl> UpdateAssetTransferAsync(AssetTransferIssueDtl assetTransferIssueDtl);


        
    }
}