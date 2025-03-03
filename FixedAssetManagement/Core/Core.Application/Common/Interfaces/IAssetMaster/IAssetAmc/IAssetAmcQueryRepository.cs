using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetAmc
{
    public interface IAssetAmcQueryRepository
    {
        Task<List<ExistingVendorDetails>> GetVendorDetails(string OldUnitId,string? VendorCode);
        Task<List<Core.Domain.Entities.MiscMaster>> GetRenewStatusAsync(); 
        Task<List<Core.Domain.Entities.MiscMaster>> GetCoverageScopeAsync(); 
        Task<AssetAmc?> GetByIdAsync(int Id);
        Task<(List<AssetAmc>,int)> GetAllAssetAmcAsync(int PageNumber, int PageSize, string? SearchTerm);
     

    }
}