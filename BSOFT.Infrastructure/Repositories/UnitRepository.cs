using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class UnitRepository : IUnitRepository
    {
    private readonly ApplicationDbContext _applicationDbContext;

        public UnitRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Unit>> GetAllUnitsAsync()
        {
            return await _applicationDbContext.Unit.ToListAsync();
        }

        public async Task<Unit> GetByIdAsync(int id)
        {
            return await _applicationDbContext.Unit.AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Unit> CreateAsync(Unit unit)
        {
            await _applicationDbContext.Unit.AddAsync(unit);
            await _applicationDbContext.SaveChangesAsync();
            return unit;
        }

        public async Task<int>UpdateAsync(int id, Unit unit)
        {
            var existingunit = await _applicationDbContext.Unit.FirstOrDefaultAsync(u => u.Id == id);
            if (existingunit != null)
            {
                existingunit.UnitName = unit.UnitName;
                existingunit.ShortName = unit.ShortName;
  

                existingunit.IsActive = unit.IsActive;
                _applicationDbContext.Unit.Update(existingunit);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }

        public async Task<int> DeleteAsync(int id,Unit unit)
        {
            var unitToDelete = await _applicationDbContext.Unit.FirstOrDefaultAsync(u => u.Id == id);
            if (unitToDelete != null)
            {
                unitToDelete.IsActive = unit.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
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