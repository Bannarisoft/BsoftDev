using Core.Application.Common.Interface;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;

namespace BSOFT.Infrastructure.Repositories
{
    
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CountryRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }



        public async Task<Countries> CreateAsync(Countries countries)
        {
            await _applicationDbContext.Countries.AddAsync(countries);
            await _applicationDbContext.SaveChangesAsync();
            return countries;
        }

        public async Task<int> DeleteAsync(int id, Countries country)
        {
             var CountryToDelete = await _applicationDbContext.Countries.FirstOrDefaultAsync(u => u.Id == id);
            if (CountryToDelete != null)
            {
                CountryToDelete.IsActive = country.IsActive;
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }
        public async Task<List<Countries>> GetAllCountriesAsync()
        {
            return await _applicationDbContext.Countries.ToListAsync();            
        }

        public async Task<Countries> GetByIdAsync(int id)
        {
              return await _applicationDbContext.Countries.AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Countries>> GetByCountryNameAsync(string searchPattern)
        {
            return await _applicationDbContext.Countries
            .Where(c => c.CountryName.Contains(searchPattern, StringComparison.OrdinalIgnoreCase) || c.CountryCode.Contains(searchPattern) )
            .OrderBy(c => c.CountryName)
            .ToListAsync();               
        }

        public async Task<int> UpdateAsync(int id, Countries country)
        {
            var existingCountry = await _applicationDbContext.Countries.FirstOrDefaultAsync(u => u.Id == id);
            if (existingCountry != null)
            {
                existingCountry.CountryName = country.CountryName;
                existingCountry.CountryCode = country.CountryCode;                
                
                _applicationDbContext.Countries.Update(existingCountry);
                return await _applicationDbContext.SaveChangesAsync();
            }
           return 0; // No user found
        }

    }
}