using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using System.Text;

namespace Core.Application.Common.Interfaces.ICompany
{
    public interface ICompanyQueryRepository
    {
        Task<(List<Company>,int)> GetAllCompaniesAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<Company> GetByIdAsync(int id);
        Task<List<Company>> GetCompany(string searchPattern);
        Task<Company?> GetByCompanynameAsync(string name,int? id = null);
       
    }
}