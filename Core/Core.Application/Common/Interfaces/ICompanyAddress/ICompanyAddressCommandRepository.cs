using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ICompanyAddress
{
    public interface ICompanyAddressCommandRepository
    {        
        Task<CompanyAddress> CreateAsync(CompanyAddress companyAddress);     
        Task<int> UpdateAsync(int id,CompanyAddress companyAddress);
        Task<int> DeleteAsync(int id,CompanyAddress companyAddress);        
    }
}