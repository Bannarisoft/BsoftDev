using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Domain.Common;
using Core.Domain.Entities;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.DepreciationDetail
{
    public class DepreciationDetailCommandRepository : IDepreciationDetailCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DepreciationDetailCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        
        public async Task<int> DeleteAsync(int companyId, int unitId, string finYear, string depreciationType,int depreciationPeriod)
        {
            var depreciationDetails = await _applicationDbContext.DepreciationDetails
                .Where(d => d.CompanyId == companyId &&
                                           (unitId == 0 || d.UnitId == unitId) &&
                                          d.Finyear == finYear &&
                                          d.DepreciationPeriod == depreciationPeriod && d.DepreciationType==depreciationType ).ToListAsync();        
            if (depreciationDetails != null)
            {                 
                foreach (var detail in depreciationDetails)
                {
                    detail.IsDeleted = BaseEntity.IsDelete.Deleted;
                }

                return await _applicationDbContext.SaveChangesAsync();
            }   
            return 0;
        }
    }
}
