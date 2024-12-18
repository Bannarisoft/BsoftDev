using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Entities;
using System.Text;

namespace BSOFT.Application.Common.Interfaces
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllCompaniesAsync();
        Task<Company> CreateAsync(Company company);
        Task<Company> GetByIdAsync(int id);
        Task<int> UpdateAsync(int id,Company company);
        Task<int> DeleteAsync(int id,Company company);
        Task<List<Company>> GetCompany(string searchPattern);
       
    }
}