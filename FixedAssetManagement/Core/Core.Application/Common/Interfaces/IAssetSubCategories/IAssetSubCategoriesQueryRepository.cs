using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetSubCategories.Queries.GetAssetSubCategories;

namespace Core.Application.Common.Interfaces.IAssetSubCategories
{
    public interface IAssetSubCategoriesQueryRepository
    {
    Task<Core.Domain.Entities.AssetSubCategories?> GetByIdAsync(int Id);
    Task<(List<Core.Domain.Entities.AssetSubCategories>,int)> GetAllAssetSubCategoriesAsync(int PageNumber, int PageSize, string? SearchTerm);
    Task<List<Core.Domain.Entities.AssetSubCategories>> GetAssetSubCategories(string searchPattern);
    Task<List<AssetSubCategoriesAutoCompleteDto?>> GetSubcategoriesByAssetCategoryIdAsync(int AssetCategoriesId);
    }
}