using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Domain.Interfaces
{
    public interface IUnitRepository
    {
       Task<List<Unit>> GetAllUnitsAsync();
       Task<Unit> GetByIdAsync(int Id);
       Task<Unit> CreateAsync(Unit unit);
       Task<int> UpdateAsync(int Id,Unit unit);
       Task<int> DeleteAsync(int Id,Unit unit);
       Task<List<Unit>> GetUnit(string searchPattern);
    }
}