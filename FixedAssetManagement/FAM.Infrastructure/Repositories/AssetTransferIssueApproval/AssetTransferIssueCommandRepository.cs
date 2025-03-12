using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IAssetTransferIssueApproval;
using Core.Domain.Entities.AssetMaster;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.AssetTransferIssueApproval
{
    public class AssetTransferIssueCommandRepository : IAssetTransferIssueApprovalCommandRepository
    {
         private readonly ApplicationDbContext _applicationDbContext;

        public AssetTransferIssueCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }


        public async Task<List<AssetTransferIssueHdr>> GetByIdsAsync(List<int> ids)
        {
            return await _applicationDbContext.AssetTransferIssueHdr
            .Where(x => ids.Contains(x.Id) && x.Status == "Pending") // Filter by Pending status
            .ToListAsync();
        }

        public async Task<int> UpdateRangeAsync(List<AssetTransferIssueHdr> transfers)
        {
            _applicationDbContext.AssetTransferIssueHdr.UpdateRange(transfers);
            return await _applicationDbContext.SaveChangesAsync(); // Return number of affected rows
        }
    }
}