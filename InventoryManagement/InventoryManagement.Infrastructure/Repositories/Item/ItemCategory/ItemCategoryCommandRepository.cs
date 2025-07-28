
using Core.Application.Common.Interfaces.Item.ItemCategory;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemCategory
{
    public class ItemCategoryCommandRepository : IItemCategoryCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ItemCategoryCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<int> CreateAsync(Core.Domain.Entities.Item.ItemCategory itemCategory)
        {                
            await _applicationDbContext.ItemCategory.AddAsync(itemCategory);
            await _applicationDbContext.SaveChangesAsync();                
            return itemCategory.Id;
        }

        public async Task<int> DeleteAsync(int Id, Core.Domain.Entities.Item.ItemCategory itemCategory)
        {            
            var itemCategoryToDelete = await _applicationDbContext.ItemCategory.FirstOrDefaultAsync(u => u.Id == Id);            
            if (itemCategoryToDelete is null)
            {
                return -1;
            }            
            itemCategoryToDelete.IsDeleted = itemCategory.IsDeleted;            
            await _applicationDbContext.SaveChangesAsync();
            return 1; 
        }
        public async Task<int> UpdateAsync(int Id, Core.Domain.Entities.Item.ItemCategory itemCategory)
        {
            var existingItemCategory = await _applicationDbContext.ItemCategory.FirstOrDefaultAsync(u => u.Id == Id);          
            if (existingItemCategory is null)
            {
                return -1;
            }            
            existingItemCategory.ItemCategoryName = itemCategory.ItemCategoryName;
            existingItemCategory.ItemGroupId = itemCategory.ItemGroupId;            
            existingItemCategory.IsGroup = itemCategory.IsGroup;    
            existingItemCategory.ParentCategoryId = itemCategory.ParentCategoryId;    
            existingItemCategory.IsBudgetApplicable = itemCategory.IsBudgetApplicable;  
            existingItemCategory.IsActive=itemCategory.IsActive;
            
            _applicationDbContext.ItemCategory.Update(existingItemCategory);
            
            await _applicationDbContext.SaveChangesAsync();
            return 1;
        }
        public async Task<bool> IsNameDuplicateAsync(string? name, int itemGroupId,int excludeId)
        {
            return await _applicationDbContext.ItemCategory
                .Where(cc => cc.ItemCategoryName == name && cc.ItemGroupId == itemGroupId && cc.Id != excludeId)
                .AnyAsync();
        }
    }
}