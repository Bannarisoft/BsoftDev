using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces
{
    public interface ICompanyAddressRepository
    {
        Task<List<CompanyAddress>> GetAllCompaniesAsync();
        Task<CompanyAddress> CreateAsync(CompanyAddress companyAddress);
        Task<CompanyAddress> GetByIdAsync(int id);
        Task<int> UpdateAsync(int id,CompanyAddress companyAddress);
        Task<int> DeleteAsync(int id,CompanyAddress companyAddress);
    }
}