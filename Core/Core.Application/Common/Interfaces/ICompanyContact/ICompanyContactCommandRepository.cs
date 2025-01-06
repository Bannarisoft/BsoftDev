using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ICompanyContact
{
    public interface ICompanyContactCommandRepository
    {        
        Task<CompanyContact> CreateAsync(CompanyContact companyContact);     
        Task<int> UpdateAsync(int id,CompanyContact companyContact);
        Task<int> DeleteAsync(int id,CompanyContact companyContact);
    }
}