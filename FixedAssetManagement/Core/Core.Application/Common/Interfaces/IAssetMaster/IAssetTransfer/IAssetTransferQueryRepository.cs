using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransfer.Queries.GetAssetTransfered;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetTransfer
{
    public interface IAssetTransferQueryRepository
    {
       
        // Task<(List<Core.Domain.Entities.AssetMaster.AssetTransfer>,int)> GetAllAsync(int PageNumber, int PageSize, string? SearchTerm);


          Task<(List<AssetTransferDto>,int)> GetAllAsync(int PageNumber, int PageSize, string? SearchTerm);
           //Task<AssetTransferDto>  GetByIdAsync(int assetId);
        
    }
}