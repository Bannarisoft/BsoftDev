using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Application.Common.Interfaces.ICity;

namespace BSOFT.Infrastructure.Repositories.City
{
    public class CityCommandRepository : ICityCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;        
        public CityCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;            
        }
        public async Task<Cities> CreateAsync(Cities cities)
        {
            await _applicationDbContext.Cities.AddAsync(cities);
            await _applicationDbContext.SaveChangesAsync();
            return cities;
        }
        public async Task<int> DeleteAsync(int id, Cities cities)
        {
            var CityToDelete = await _applicationDbContext.Cities.FirstOrDefaultAsync(u => u.Id == id);
            if (CityToDelete != null)
            {
                CityToDelete.IsActive = cities.IsActive;              
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0; // No user found
        }
        public async Task<int> UpdateAsync(int id, Cities city)
        {
            var existingCity = await _applicationDbContext.Cities.FirstOrDefaultAsync(u => u.Id == id);             
    
            if (existingCity != null)
            {
                existingCity.CityName = city.CityName;
                existingCity.CityCode = city.CityCode;                
                existingCity.StateId = city.StateId;                
                
                _applicationDbContext.Cities.Update(existingCity);
                return await _applicationDbContext.SaveChangesAsync();
            }
           return 0; 
        }
        public async Task<bool> StateExistsAsync(int stateId)
        {        
            return await _applicationDbContext.States.AnyAsync(s => s.Id == stateId && s.IsActive == 1);
        }
        public async Task<bool> GetCityByCodeAsync(string cityCode, int stateId)
        {
            return await _applicationDbContext.Cities
            .AnyAsync(c => c.CityCode == cityCode && c.StateId == stateId); // Checks both CityCode and StateId
        }
    }
}