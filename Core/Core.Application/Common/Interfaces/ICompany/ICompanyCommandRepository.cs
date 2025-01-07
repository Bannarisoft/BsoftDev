using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using System.Text;

namespace Core.Application.Common.Interfaces.ICompany
{
    public interface ICompanyCommandRepository
    {
        Task<Company> CreateAsync(Company company);
        Task<int> UpdateAsync(int id,Company company);
        Task<int> DeleteAsync(int id,Company company);      
    }
}