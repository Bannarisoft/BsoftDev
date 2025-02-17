using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMiscMaster;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.MiscMaster
{
    public class MiscMasterCommandRepository : IMiscMasterCommandRepository
    {
         private readonly ApplicationDbContext _dbContext;
      

          public MiscMasterCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }

        public async   Task<Core.Domain.Entities.MiscMaster> CreateAsync(Core.Domain.Entities.MiscMaster miscMaster)
        {

             // Auto-generate SortOrder
               miscMaster.SortOrder = await GetMaxSortOrderAsync() + 1;
             await _dbContext.MiscMaster.AddAsync(miscMaster);
                await _dbContext.SaveChangesAsync();
                return miscMaster;
        }  
          public async Task<int> GetMaxSortOrderAsync()
        {
            return await _dbContext.MiscMaster.MaxAsync(ac => (int?)ac.SortOrder) ?? -1;
        } 

          public async Task<bool> UpdateAsync(int id,Core.Domain.Entities.MiscMaster miscMaster)
        {
            var existingMiscMaster =await _dbContext.MiscMaster.FirstOrDefaultAsync(m =>m.Id == miscMaster.Id);
         
            if (existingMiscMaster != null)
            {
                existingMiscMaster.Code = miscMaster.Code;
                existingMiscMaster.MiscTypeMasterId = miscMaster.MiscTypeMasterId;
                existingMiscMaster.Description = miscMaster.Description; 
                existingMiscMaster.SortOrder = miscMaster.SortOrder;              
                existingMiscMaster.IsActive = miscMaster.IsActive;

                _dbContext.MiscMaster.Update(existingMiscMaster);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            return false;
        }
        
          public async Task<bool> DeleteAsync(int id,Core.Domain.Entities.MiscMaster miscMaster)
        {
            var existingMiscmaster = await _dbContext.MiscMaster.FirstOrDefaultAsync(u => u.Id == id);
            if (existingMiscmaster != null)
            {
                existingMiscmaster.IsDeleted = miscMaster.IsDeleted;
                return await _dbContext.SaveChangesAsync() >0;
            }
            return false; 
        }        



    }
}