using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetSubCategories
{
    public interface IAssetSubCategoriesQueryRepository
    {
    Task<Core.Domain.Entities.AssetSubCategories?> GetByIdAsync(int Id);
    Task<(List<Core.Domain.Entities.AssetSubCategories>,int)> GetAllAssetSubCategoriesAsync(int PageNumber, int PageSize, string? SearchTerm);
    Task<List<Core.Domain.Entities.AssetSubCategories>> GetAssetSubCategories(string searchPattern);
    }
}