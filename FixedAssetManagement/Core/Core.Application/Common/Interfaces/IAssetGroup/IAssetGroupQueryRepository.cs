using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetGroup
{
    public interface IAssetGroupQueryRepository
    {
    Task<Core.Domain.Entities.AssetGroup?> GetByIdAsync(int Id);
    Task<(List<Core.Domain.Entities.AssetGroup>,int)> GetAllAssetGroupAsync(int PageNumber, int PageSize, string? SearchTerm);
    Task<List<Core.Domain.Entities.AssetGroup>> GetAssetGroups(string searchPattern);
     
    }
}