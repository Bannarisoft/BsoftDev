using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUnit;

namespace BSOFT.Infrastructure.Repositories.Units
{
    public class UnitQueryRepository : IUnitQueryRepository
    {
    private readonly ApplicationDbContext _applicationDbContext;

        public UnitQueryRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Unit>> GetAllUnitsAsync()
        {
            return await _applicationDbContext.Unit.ToListAsync();
        }

        public async Task<Unit> GetByIdAsync(int id)
        {
            return await _applicationDbContext.Unit.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        }


         public async Task<List<Unit>> GetUnit(string searchPattern = null)
        {
                       return await _applicationDbContext.Unit
                 .Where(r => EF.Functions.Like(r.UnitName, $"%{searchPattern}%")) 
                 .Select(r => new Unit
                 {
                     Id = r.Id,
                     UnitName = r.UnitName
                 })
                 .ToListAsync();
        }
        
    }
}