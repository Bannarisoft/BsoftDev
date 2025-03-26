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

        
        public async Task<int> DeleteAsync(int companyId, int unitId, int finYearId, int depreciationType,int depreciationPeriod)
        {
            var depreciationDetails = await _applicationDbContext.DepreciationDetails
                .Where(d => d.CompanyId == companyId &&
                                           (unitId == 0 || d.UnitId == unitId) &&
                                          d.Finyear == finYearId &&
                                          d.DepreciationPeriod == depreciationPeriod && d.DepreciationType==depreciationType ).ToListAsync();        
            if (depreciationDetails != null)
            {
                _applicationDbContext.DepreciationDetails.RemoveRange(depreciationDetails); // Physically delete
                return await _applicationDbContext.SaveChangesAsync();                
            }   
            return 0;
        }

        public async Task<int> UpdateAsync(int companyId, int unitId, int finYearId, int depreciationType, int depreciationPeriod)
        {
            var depreciationDetails = await _applicationDbContext.DepreciationDetails
                .Where(d => d.CompanyId == companyId &&
                                           (unitId == 0 || d.UnitId == unitId) &&
                                          d.Finyear == finYearId &&
                                          d.DepreciationPeriod == depreciationPeriod && d.DepreciationType==depreciationType ).ToListAsync();        
            if (depreciationDetails != null)
            {                 
                 foreach (var detail in depreciationDetails)
                {
                    detail.IsLocked =1;
                }                              
                return await _applicationDbContext.SaveChangesAsync();                
            }   
            return 0;
        }
    }
}
