using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class ModuleRepository : IModuleRepository
    {
    private readonly ApplicationDbContext _applicationDbContext;

    public ModuleRepository(ApplicationDbContext applicationDbContext)
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

    public async Task AddModuleAsync(Modules module)
    {
        await _applicationDbContext.Modules.AddAsync(module);
    }

    public async Task SaveChangesAsync()
    {
        await _applicationDbContext.SaveChangesAsync();
    }

    public async Task UpdateModuleAsync(Modules module)
    {
        var existingModule = await _applicationDbContext.Modules.Include(m => m.Menus).FirstOrDefaultAsync(m => m.Id == module.Id);

        if (existingModule != null)
        {
            existingModule.ModuleName = module.ModuleName;

            // Update menus
            foreach (var menu in module.Menus)
            {
                var existingMenu = existingModule.Menus.FirstOrDefault(m => m.Id == menu.Id);
                if (existingMenu != null)
                {
                    existingMenu.MenuName = menu.MenuName;
                }
            }
        }
    }

    public async Task DeleteModuleAsync(int moduleId)
    {
        var module = await _applicationDbContext.Modules.Include(m => m.Menus).FirstOrDefaultAsync(m => m.Id == moduleId);

        if (module != null)
        {
            module.IsDeleted = true;

            foreach (var menu in module.Menus)
            {
                menu.IsDeleted = true;
            }
        }
    }

    }
}