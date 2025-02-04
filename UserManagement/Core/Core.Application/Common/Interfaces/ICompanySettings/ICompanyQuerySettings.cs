using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.ICompanySettings
{
    public interface ICompanyQuerySettings
    {
        Task<Core.Domain.Entities.CompanySettings> GetByIdAsync(int id);
    }
}