using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetDisposal
{
    public interface IAssetDisposalQueryRepository
    {
         Task<List<Core.Domain.Entities.MiscMaster>> GetDisposalType(); 
         Task<AssetDisposal?> GetByIdAsync(int Id);
         Task<(List<AssetDisposal>,int)> GetAllAssetDisposalAsync(int PageNumber, int PageSize, string? SearchTerm); 
    }
}