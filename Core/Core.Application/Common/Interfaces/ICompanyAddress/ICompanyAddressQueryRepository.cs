using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ICompanyAddress
{
    public interface ICompanyAddressQueryRepository
    {           
        Task<List<CompanyAddress>> GetAllCompaniesAsync();        
        Task<CompanyAddress> GetByIdAsync(int id);        
    }
}