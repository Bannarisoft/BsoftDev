using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IEntity;


namespace BSOFT.Infrastructure.Repositories.Entities
{
    public class EntityQueryRepository : IEntityQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public EntityQueryRepository(ApplicationDbContext applicationDbContext)
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
        public async Task<List<Entity>> GetByEntityNameAsync(string searchPattern)
        {
            return await _applicationDbContext.Entity
            .Where(c => c.EntityName.Contains(searchPattern, StringComparison.OrdinalIgnoreCase) || c.EntityCode.Contains(searchPattern) )
            .OrderBy(c => c.EntityName)
            .ToListAsync();               
        }

        
    }
}