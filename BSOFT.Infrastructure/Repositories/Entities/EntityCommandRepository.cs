using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IEntity;


namespace BSOFT.Infrastructure.Repositories.Entities
{
    public class EntityCommandRepository : IEntityCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public EntityCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
    
       public async Task<Entity> CreateAsync(Entity entity)
        {
            await _applicationDbContext.Entity.AddAsync(entity);
            await _applicationDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<int>UpdateAsync(int id, Entity entity)
        {
            var existingentity = await _applicationDbContext.Entity.FirstOrDefaultAsync(u => u.Id == id);
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
            var EntityToDelete = await _applicationDbContext.Entity.FirstOrDefaultAsync(u => u.Id == id);
            if (EntityToDelete != null)
            {
                EntityToDelete.IsActive = entity.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }

    }
}