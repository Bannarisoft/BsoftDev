using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetCategories
{
    public interface IAssetCategoriesQueryRepository 
    {
    Task<Core.Domain.Entities.AssetCategories?> GetByIdAsync(int Id);
    Task<(List<Core.Domain.Entities.AssetCategories>,int)> GetAllAssetCategoriesAsync(int PageNumber, int PageSize, string? SearchTerm);
    Task<List<Core.Domain.Entities.AssetCategories>> GetAssetCategories(string searchPattern);
    }
}