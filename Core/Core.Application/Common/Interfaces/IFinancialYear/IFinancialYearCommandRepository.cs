using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IFinancialYear
{
    public interface IFinancialYearCommandRepository
    {

        Task<int> CreateAsync(Core.Domain.Entities.FinancialYear financialYear);
        Task<int> UpdateAsync(int id, Core.Domain.Entities.FinancialYear financialYear);
     
        
    }
}