using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IModule;

namespace UserManagement.Infrastructure.Repositories.Module
{
    public class ModuleCommandRepository : IModuleCommandRepository
    {
    private readonly ApplicationDbContext _applicationDbContext;

    public ModuleCommandRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }   

    public async Task AddModuleAsync(Modules module)
    {
       // Check for existing module name before adding
            bool moduleExists = await _applicationDbContext.Modules
                .AnyAsync(m => m.ModuleName == module.ModuleName && !m.IsDeleted);

            if (moduleExists)
            {
                throw new InvalidOperationException($"A module with the name '{module.ModuleName}' already exists.");
            }

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
        // var module = await _applicationDbContext.Modules.Include(m => m.Menus).FirstOrDefaultAsync(m => m.Id == moduleId);

        // if (module != null)
        // {
        //     module.IsDeleted = true;

        //     foreach (var menu in module.Menus)
        //     {
        //         menu.IsDeleted = true;
        //     }
        // }
    }

    }
}