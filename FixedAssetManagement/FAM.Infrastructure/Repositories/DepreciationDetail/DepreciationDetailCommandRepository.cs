using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Domain.Entities;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FAM.Infrastructure.Repositories.DepreciationDetail
{
    public class DepreciationDetailCommandRepository : IDepreciationDetailCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DepreciationDetailCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        
        public async Task<int> DeleteAsync(int companyId, int unitId, string finYear, DateTimeOffset startDate, DateTimeOffset endDate, string depreciationType)
        {
            var entity = await _applicationDbContext.DepreciationDetails
                .FirstOrDefaultAsync(d => d.CompanyId == companyId &&
                                          d.UnitId == unitId &&
                                          d.Finyear == finYear &&
                                          d.StartDate == startDate &&
                                          d.EndDate == endDate && d.DepreciationType==depreciationType);
            if (entity != null)
            {
                _applicationDbContext.DepreciationDetails.Remove(entity);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0;
        }
    }
}
