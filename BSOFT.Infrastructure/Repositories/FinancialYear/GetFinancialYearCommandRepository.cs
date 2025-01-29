using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces.IFinancialYear;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;

namespace BSOFT.Infrastructure.Repositories.FinancialYear
{
    public class GetFinancialYearCommandRepository :IFinancialYearCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public GetFinancialYearCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;

        }
        public async Task<Core.Domain.Entities.FinancialYear> CreateAsync(Core.Domain.Entities.FinancialYear financialYear)
         {
            await _applicationDbContext.FinancialYear.AddAsync(financialYear);
            await _applicationDbContext.SaveChangesAsync();
            return financialYear;
            }


    }
}