using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IAssetGroup
{
    public interface IAssetGroupCommandRepository
    {
         Task<int> CreateAsync(Core.Domain.Entities.AssetGroup assetGroup);
         Task<bool> ExistsByCodeAsync(string code );
         Task<int> GetMaxSortOrderAsync();
       
    //   Task<int> UpdateAsync(int Id,AssetGroup assetGroup);
    //   Task<int> DeleteEntityAsync(int Id,AssetGroup assetGroup);
    //   Task<bool> ExistsByCodeAsync(string assetGroup); // Check if code exists
    //   Task<bool> ExistsByNameupdateAsync(string name,int id );
    }
}