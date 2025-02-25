using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetLocation
{
    public interface IAssetLocationQueryRepository
    {
            Task<(List<Core.Domain.Entities.AssetMaster.AssetLocation>,int)> GetAllAssetLocationAsync(int PageNumber, int PageSize, string? SearchTerm);
          
             Task<Core.Domain.Entities.AssetMaster.AssetLocation>  GetByIdAsync(int id);
            Task<Core.Domain.Entities.AssetMaster.AssetLocation?> GetByAssetLocationCodeAsync(int? id = null);

              Task<(List<Core.Domain.Entities.AssetMaster.Employee>,int)> GetAllCustodianAsync(string OldUnitId, string? SearchTerm);


    }
}