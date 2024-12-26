using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Common.Interfaces
{
    public interface IModuleRepository
    {
    Task<BSOFT.Domain.Entities.Modules> GetModuleByIdAsync(int id);
    Task<List<BSOFT.Domain.Entities.Modules>> GetAllModulesAsync();
    Task AddModuleAsync(BSOFT.Domain.Entities.Modules module);
    Task SaveChangesAsync(); 
    Task DeleteModuleAsync(int moduleId);
    Task UpdateModuleAsync(BSOFT.Domain.Entities.Modules module);
    }
}