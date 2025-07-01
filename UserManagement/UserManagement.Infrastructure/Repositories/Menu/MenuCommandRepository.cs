using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMenu;
using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Repositories.Menu
{
    public class MenuCommandRepository : IMenuCommand
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public MenuCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<bool> BulkImportMenuAsync(List<Core.Domain.Entities.Menu> menus)
        {
            await _applicationDbContext.Menus.AddRangeAsync(menus);
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> CreateAsync(Core.Domain.Entities.Menu menu)
        {
            await _applicationDbContext.Menus.AddAsync(menu);
            await _applicationDbContext.SaveChangesAsync();
            return menu.Id;
        }

        public async Task<bool> DeleteAsync(int id, Core.Domain.Entities.Menu menu)
        {
            var existingMenu = await _applicationDbContext.Divisions.FirstOrDefaultAsync(u => u.Id == id);
            if (existingMenu != null)
            {
                existingMenu.IsDeleted = menu.IsDeleted;
                return await _applicationDbContext.SaveChangesAsync() >0;
            }
            return false; 
        }

        public async Task<bool> UpdateAsync(Core.Domain.Entities.Menu menu)
        {
            var existingMenu = await _applicationDbContext.Menus.FirstOrDefaultAsync(u => u.Id == menu.Id);
            if (existingMenu != null)
            {
                existingMenu.MenuName = menu.MenuName;
                existingMenu.ModuleId = menu.ModuleId;
                existingMenu.MenuIcon = menu.MenuIcon;
                existingMenu.MenuUrl = menu.MenuUrl;
                existingMenu.ParentId = menu.ParentId;
                existingMenu.SortOrder = menu.SortOrder;
                existingMenu.MenuIcon = menu.MenuIcon;
                existingMenu.IsActive = menu.IsActive;

                _applicationDbContext.Menus.Update(existingMenu);
                return await _applicationDbContext.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}