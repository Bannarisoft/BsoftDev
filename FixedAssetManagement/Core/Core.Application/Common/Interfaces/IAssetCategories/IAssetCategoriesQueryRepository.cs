using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetCategories.Queries.GetAssetCategories;

namespace Core.Application.Common.Interfaces.IAssetCategories
{
    public interface IAssetCategoriesQueryRepository 
    {
    Task<AssetCategoriesDto?> GetByIdAsync(int Id);
    Task<(List<AssetCategoriesDto>,int)> GetAllAssetCategoriesAsync(int PageNumber, int PageSize, string? SearchTerm);
    Task<List<Core.Domain.Entities.AssetCategories>> GetAssetCategories(string searchPattern);
    Task<List<AssetCategoriesAutoCompleteDto?>> GetByAssetgroupIdAsync(int AssetGroupId);
    }
}