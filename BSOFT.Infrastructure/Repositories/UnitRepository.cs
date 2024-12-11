using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Domain.Interfaces;
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
                .FirstOrDefaultAsync(b => b.UnitId == id);
        }

        public async Task<Unit> CreateAsync(Unit unit)
        {
            await _applicationDbContext.Unit.AddAsync(unit);
            await _applicationDbContext.SaveChangesAsync();
            return unit;
        }

        public async Task<int>UpdateAsync(int id, Unit unit)
        {
            var existingUser = await _applicationDbContext.Unit.FirstOrDefaultAsync(u => u.UnitId == id);
            if (existingUser != null)
            {
                existingUser.Name = unit.Name;
                existingUser.ShortName = unit.ShortName;
                existingUser.Address1 = unit.Address1;

                existingUser.Address2 = unit.Address2;
                existingUser.Address3 = unit.Address3;
                existingUser.CoId = unit.CoId;

                existingUser.DivId = unit.DivId;
                existingUser.UnitHeadName = unit.UnitHeadName;
                existingUser.Mobile = unit.Mobile;

                existingUser.Email = unit.Email;
                existingUser.IsActive = unit.IsActive;
                existingUser.ModifiedBy = unit.ModifiedBy;
                existingUser.ModifiedByName = unit.ModifiedByName;

                existingUser.ModifiedAt = unit.ModifiedAt;
                existingUser.ModifiedIP = unit.ModifiedIP;




                _applicationDbContext.Unit.Update(existingUser);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }

        public async Task<int> DeleteAsync(int id)
        {
            var unitToDelete = await _applicationDbContext.Unit.FirstOrDefaultAsync(u => u.UnitId == id);
            if (unitToDelete != null)
            {
                _applicationDbContext.Unit.Remove(unitToDelete);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }
    }
}