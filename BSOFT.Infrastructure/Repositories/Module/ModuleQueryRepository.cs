using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IModule;

namespace BSOFT.Infrastructure.Repositories.Module
{
    public class ModuleQueryRepository : IModuleQueryRepository
    {
    private readonly ApplicationDbContext _applicationDbContext;

    public ModuleQueryRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
     public async Task<Modules> GetModuleByIdAsync(int id)
    {
        return await _applicationDbContext.Modules.Include(m => m.Menus).FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Modules>> GetAllModulesAsync()
    {
        return await _applicationDbContext.Modules.Include(m => m.Menus).ToListAsync();
    }  
    }
}