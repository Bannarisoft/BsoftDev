using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using System.Text;

namespace Core.Application.Common.Interfaces
{
    public interface IDivisionRepository
    {
        Task<List<Division>> GetAllDivisionAsync();
        Task<Division> CreateAsync(Division division);
        Task<Division> GetByIdAsync(int id);
        Task<int> UpdateAsync(int id,Division division);
        Task<int> DeleteAsync(int id,Division division);
        Task<List<Division>> GetDivision(string searchPattern);
    }
}