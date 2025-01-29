using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IFinancialYear
{
    public interface IFinancialYearCommandRepository
    {

        Task< Core.Domain.Entities.FinancialYear> CreateAsync(Core.Domain.Entities.FinancialYear department);
        
    }
}