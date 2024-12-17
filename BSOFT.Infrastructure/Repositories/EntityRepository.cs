using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class EntityRepository : IEntityRepository
    {
         private readonly ApplicationDbContext _applicationDbContext;

        public EntityRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Entity>> GetAllEntityAsync()
        {
            return await _applicationDbContext.Entity.ToListAsync();
        }

        public async Task<Entity> GetByIdAsync(int id)
        {
            return await _applicationDbContext.Entity.AsNoTracking()
                .FirstOrDefaultAsync(b => b.EntityId == id);
        }
     // Last Entity Code Check 
    public async Task<string> GenerateEntityCodeAsync()
    {
      var lastCode = await _applicationDbContext.Entity
        .OrderByDescending(e => e.EntityCode)
        .ThenByDescending(e => e.EntityId)
        .Select(e => e.EntityCode)
        .FirstOrDefaultAsync() ?? "ENT-00000";

    var nextCodeNumber = int.Parse(lastCode[(lastCode.IndexOf('-') + 1)..]) + 1;

    return $"ENT-{nextCodeNumber:D5}";;
    }

       public async Task<Entity> CreateAsync(Entity entity)
        {
            await _applicationDbContext.Entity.AddAsync(entity);
            await _applicationDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<int>UpdateAsync(int id, Entity entity)
        {
            var existingentity = await _applicationDbContext.Entity.FirstOrDefaultAsync(u => u.EntityId == id);
            if (existingentity != null)
            {
                existingentity.EntityName = entity.EntityName;
                existingentity.EntityDescription = entity.EntityDescription;
                existingentity.Address = entity.Address;
              
                existingentity.Phone = entity.Phone;
                existingentity.Email = entity.Email;
                existingentity.IsActive = entity.IsActive;
                _applicationDbContext.Entity.Update(existingentity);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }

        public async Task<int> DeleteAsync(int id,Entity entity)
        {
            var EntityToDelete = await _applicationDbContext.Entity.FirstOrDefaultAsync(u => u.EntityId == id);
            if (EntityToDelete != null)
            {
                EntityToDelete.IsActive = entity.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        } 

        
    }
}