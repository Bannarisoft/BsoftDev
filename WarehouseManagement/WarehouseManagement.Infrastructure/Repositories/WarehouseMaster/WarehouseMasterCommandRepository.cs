using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IWarehouseMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories.WarehouseMaster
{
    public class WarehouseMasterCommandRepository : IWarehouseMasterCommandRepository
    {

        private readonly ApplicationDbContext _context;

        public readonly ILogger<WarehouseMasterCommandRepository> _logger;

        public WarehouseMasterCommandRepository(ApplicationDbContext context, ILogger<WarehouseMasterCommandRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> CreateAsync(Core.Domain.Entities.WarehouseMaster warehouseMaster)
        {

            await _context.WarehouseMasters.AddAsync(warehouseMaster);

            await _context.SaveChangesAsync();

            return warehouseMaster.Id;
        }

        public async Task<int> UpdateAsync(Core.Domain.Entities.WarehouseMaster warehouseMaster)
        {
            _context.WarehouseMasters.Update(warehouseMaster);

            try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    var root = ex.GetBaseException(); // usually SqlException
                    _logger.LogError(root, "DB save failed: {Msg}", root.Message);
                    throw; // or map to API error
                }
           // await _context.SaveChangesAsync();
            return warehouseMaster.Id;
        }

        public async Task<Core.Domain.Entities.WarehouseMaster?> GetByIdAsync(int id)
        {
            return await _context.WarehouseMasters
                .Include(w => w.AllowedItemGroups)
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted);
        }

        public async Task<bool> DeleteAsync(int id, Core.Domain.Entities.WarehouseMaster warehouseMaster)
        {
            // Load current entity (not already deleted) + mappings
            var existing = await _context.WarehouseMasters
                .Include(x => x.AllowedItemGroups)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted);

            if (existing is null)
                return false;

            // Guard: block delete if it has active children
            var hasChildren = await _context.WarehouseMasters
                .AnyAsync(x => x.ParentWarehouseId == id && x.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted);

            if (hasChildren)
                throw new InvalidOperationException("Cannot delete a warehouse group that has child warehouses.");

            // Soft delete the warehouse
            existing.IsDeleted = Core.Domain.Common.BaseEntity.IsDelete.Deleted;
            existing.IsActive = Core.Domain.Common.BaseEntity.Status.Inactive;
            // existing.ModifiedDate = DateTimeOffset.UtcNow;

            // Soft delete related mappings
            if (existing.AllowedItemGroups?.Count > 0)
            {
                foreach (var m in existing.AllowedItemGroups
                            .Where(m => m.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted))
                {
                    m.IsDeleted = Core.Domain.Common.BaseEntity.IsDelete.Deleted;
                    m.IsActive = Core.Domain.Common.BaseEntity.Status.Inactive;

                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
        
       
       


    }
}