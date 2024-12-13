using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Repositories
{
    public class DivisionRepository : IDivisionRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public DivisionRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
         public async Task<List<Division>> GetAllDivisionAsync()
        {
            return await _applicationDbContext.Divisions.ToListAsync();
        }
         public async Task<Division> CreateAsync(Division division)
        {
            await _applicationDbContext.Divisions.AddAsync(division);
            await _applicationDbContext.SaveChangesAsync();
            return division;
        }
         public async Task<Division> GetByIdAsync(int id)
        {
            return await _applicationDbContext.Divisions.AsNoTracking()
                .FirstOrDefaultAsync(b => b.DivId == id);
        }
           public async Task<int> UpdateAsync(int id, Division division)
        {
            var existingDivision = await _applicationDbContext.Divisions.FirstOrDefaultAsync(u => u.DivId == id);
            if (existingDivision != null)
            {
                existingDivision.ShortName = division.ShortName;
                existingDivision.Name = division.Name;
                existingDivision.CompanyId = division.CompanyId;
                existingDivision.IsActive = division.IsActive;

                _applicationDbContext.Divisions.Update(existingDivision);
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }
         public async Task<int> DeleteAsync(int id,Division division)
        {
            var existingDivision = await _applicationDbContext.Divisions.FirstOrDefaultAsync(u => u.DivId == id);
            if (existingDivision != null)
            {
                existingDivision.IsActive = division.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }
            public async Task<List<Division>>  GetDivision(string searchPattern = null)
        {
                       return await _applicationDbContext.Divisions
                 .Where(d => EF.Functions.Like(d.Name, $"%{searchPattern}%")) 
                 .Select(d => new Division
                 {
                     DivId = d.DivId,
                     Name = d.Name
                 })
                 .ToListAsync();
        }
    }
}