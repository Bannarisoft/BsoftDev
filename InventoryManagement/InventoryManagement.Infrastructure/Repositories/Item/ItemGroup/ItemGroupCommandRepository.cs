
using Core.Application.Common.Interfaces.Item.ItemGroup;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemGroup
{
    public class ItemGroupCommandRepository : IItemGroupCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ItemGroupCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<int> CreateAsync(Core.Domain.Entities.Item.ItemGroup itemGroup)
        {                
            await _applicationDbContext.ItemGroup.AddAsync(itemGroup);
            await _applicationDbContext.SaveChangesAsync();                
            return itemGroup.Id;
        }

        public async Task<int> DeleteAsync(int Id, Core.Domain.Entities.Item.ItemGroup itemGroup)
        {            
            var itemGroupToDelete = await _applicationDbContext.ItemGroup.FirstOrDefaultAsync(u => u.Id == Id);            
            if (itemGroupToDelete is null)
            {
                return -1;
            }            
            itemGroupToDelete.IsDeleted = itemGroup.IsDeleted;            
            await _applicationDbContext.SaveChangesAsync();
            return 1; 
        }
        public async Task<int> UpdateAsync(int Id, Core.Domain.Entities.Item.ItemGroup itemGroup)
        {
            var existingItemGroup = await _applicationDbContext.ItemGroup.FirstOrDefaultAsync(u => u.Id == Id);          
            if (existingItemGroup is null)
            {
                return -1;
            }            
            existingItemGroup.ItemGroupName = itemGroup.ItemGroupName;                       
            existingItemGroup.IsActive=itemGroup.IsActive;
            
            _applicationDbContext.ItemGroup.Update(existingItemGroup);
            
            await _applicationDbContext.SaveChangesAsync();
            return 1;
        }
        public async Task<bool> IsNameDuplicateAsync(string? name, int excludeId)
        {
            return await _applicationDbContext.ItemGroup
                .Where(cc => cc.ItemGroupName == name  && cc.Id != excludeId)
                .AnyAsync();
        }
    }
}