using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ICompanyContact
{
    public interface ICompanyContactQueryRepository
    {
        Task<List<CompanyContact>> GetAllCompaniesAsync();      
        Task<CompanyContact> GetByIdAsync(int id);        
    }
}