using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.ILocation;
using Core.Domain.Entities;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.Locations
{
    public class LocationCommandRepository : ILocationCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public LocationCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Core.Domain.Entities.Location> CreateAsync(Core.Domain.Entities.Location location)
        {
            await _applicationDbContext.Locations.AddAsync(location);
            await _applicationDbContext.SaveChangesAsync();
            return location;
        }

        public async Task<bool> DeleteAsync(int id, Location location)
        {
            var existingLocation = await _applicationDbContext.Locations.FirstOrDefaultAsync(u => u.Id == id);
            if (existingLocation != null)
            {
                existingLocation.IsDeleted = location.IsDeleted;
                return await _applicationDbContext.SaveChangesAsync() >0;
            }
            return false; 
        }

        public async Task<bool> UpdateAsync(Location location)
        {
            var existingLocation = await _applicationDbContext.Locations.FirstOrDefaultAsync(u => u.Id == location.Id);
            if (existingLocation != null)
            {
                existingLocation.Code = location.Code;
                existingLocation.LocationName = location.LocationName;
                existingLocation.Description = location.Description;
                existingLocation.SortOrder = location.SortOrder;
                existingLocation.UnitId = location.UnitId;
                existingLocation.DepartmentId = location.DepartmentId;
                existingLocation.IsActive = location.IsActive;

                _applicationDbContext.Locations.Update(existingLocation);
                return await _applicationDbContext.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}