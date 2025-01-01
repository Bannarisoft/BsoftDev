using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;


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
                .FirstOrDefaultAsync(b => b.Id == id);
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

        public async Task<List<Entity>> GetByEntityNameAsync(string searchPattern)
        {
            return await _applicationDbContext.Entity
            .Where(c => c.EntityName.Contains(searchPattern, StringComparison.OrdinalIgnoreCase) || c.EntityCode.Contains(searchPattern) )
            .OrderBy(c => c.EntityName)
            .ToListAsync();               
        }

        
    }
}