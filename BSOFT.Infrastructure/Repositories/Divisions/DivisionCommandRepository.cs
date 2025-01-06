using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDivision;

namespace BSOFT.Infrastructure.Repositories.Divisions
{
    public class DivisionCommandRepository : IDivisionCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public DivisionCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }       
         public async Task<Division> CreateAsync(Division division)
        {
            await _applicationDbContext.Divisions.AddAsync(division);
            await _applicationDbContext.SaveChangesAsync();
            return division;
        }
        public async Task<int> UpdateAsync(int id, Division division)
        {
            var existingDivision = await _applicationDbContext.Divisions.FirstOrDefaultAsync(u => u.Id == id);
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
            var existingDivision = await _applicationDbContext.Divisions.FirstOrDefaultAsync(u => u.Id == id);
            if (existingDivision != null)
            {
                existingDivision.IsActive = division.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }        
    }
}