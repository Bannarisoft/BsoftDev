using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IAssetGroup;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.AssetGroup
{
    public class AssetGroupCommandRepository : IAssetGroupCommandRepository
    {
         private readonly ApplicationDbContext _applicationDbContext;
         
        public AssetGroupCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _applicationDbContext.AssetGroup.AnyAsync(c => c.Code == code);
        }
        
        public async Task<int> CreateAsync(Core.Domain.Entities.AssetGroup assetGroup)
        {
         // Auto-generate SortOrder
        assetGroup.SortOrder = await GetMaxSortOrderAsync() + 1;
        // Add the AssetGroup to the DbContext
        await _applicationDbContext.AssetGroup.AddAsync(assetGroup);

        // Save changes to the database
        await _applicationDbContext.SaveChangesAsync();

        // Return the ID of the created AssetGroup
        return assetGroup.Id;
        }

        public async Task<int> GetMaxSortOrderAsync()
        {
            return await _applicationDbContext.AssetGroup.MaxAsync(ac => (int?)ac.SortOrder) ?? -1;
        }
    }
}