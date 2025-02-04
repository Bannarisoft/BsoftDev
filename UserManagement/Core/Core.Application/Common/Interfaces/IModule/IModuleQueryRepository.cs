using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IModule
{
    public interface IModuleQueryRepository
    {
    Task<Core.Domain.Entities.Modules> GetModuleByIdAsync(int id);
    Task<List<Core.Domain.Entities.Modules>> GetAllModulesAsync();    
    }
}