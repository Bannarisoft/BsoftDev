using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces
{
    public interface ICompanyContactRepository
    {
        Task<List<CompanyContact>> GetAllCompaniesAsync();
        Task<CompanyContact> CreateAsync(CompanyContact companyContact);
        Task<CompanyContact> GetByIdAsync(int id);
        Task<int> UpdateAsync(int id,CompanyContact companyContact);
        Task<int> DeleteAsync(int id,CompanyContact companyContact);
    }
}