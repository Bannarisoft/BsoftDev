using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IRackMaster;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories.RackMaster
{
    public class RackMasterCommandRepository : IRackMasterCommandRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RackMasterCommandRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateAsync(Core.Domain.Entities.RackMaster rackMaster)
        {

            await _dbContext.RackMasters.AddAsync(rackMaster);
            await _dbContext.SaveChangesAsync();
            return rackMaster.Id;
        }

        public async Task<int> UpdateAsync(Core.Domain.Entities.RackMaster rackMaster)
        {
            _dbContext.RackMasters.Update(rackMaster);
            await _dbContext.SaveChangesAsync();
            return rackMaster.Id;
        }
        
            public async Task<Core.Domain.Entities.RackMaster?> GetByIdAsync(int id)
        {
            return await _dbContext.RackMasters
                .FirstOrDefaultAsync(r => r.Id == id && r.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted); 
        }
        
         public async Task<bool> DeleteAsync(int id,Core.Domain.Entities.RackMaster rackMaster)
        {
            // Load current entity (not already deleted) + mappings
            var existing = await _dbContext.RackMasters               
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted);

            if (existing is null)
                return false;                    
          
            existing.IsDeleted = Core.Domain.Common.BaseEntity.IsDelete.Deleted;
            existing.IsActive = Core.Domain.Common.BaseEntity.Status.Inactive;
         
           

            await _dbContext.SaveChangesAsync();
            return true;
        }

        
    }
}