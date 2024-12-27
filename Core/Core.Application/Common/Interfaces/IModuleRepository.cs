using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces
{
    public interface IModuleRepository
    {
    Task<Core.Domain.Entities.Modules> GetModuleByIdAsync(int id);
    Task<List<Core.Domain.Entities.Modules>> GetAllModulesAsync();
    Task AddModuleAsync(Core.Domain.Entities.Modules module);
    Task SaveChangesAsync(); 
    Task DeleteModuleAsync(int moduleId);
    Task UpdateModuleAsync(Core.Domain.Entities.Modules module);
    }
}