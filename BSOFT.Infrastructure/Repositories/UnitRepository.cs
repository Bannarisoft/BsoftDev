using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;


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
            return await _applicationDbContext.Unit.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        }

       public async Task<int> CreateUnitAsync(Unit unit)
       {
        _applicationDbContext.Unit.Add(unit);
        await _applicationDbContext.SaveChangesAsync();
        return unit.Id;
    }

       public async Task<UnitAddress> CreateUnitAddressAsync(UnitAddress unitAddress)
       {
        _applicationDbContext.UnitAddress.Add(unitAddress);
        await _applicationDbContext.SaveChangesAsync();
        return unitAddress;
       }

        public async Task<UnitContacts> CreateUnitContactsAsync(UnitContacts unitContacts)
        {
        _applicationDbContext.UnitContacts.Add(unitContacts);
        await _applicationDbContext.SaveChangesAsync();
        return unitContacts;
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


        public async Task UpdateUnitAddressAsync(int Id, UnitAddress unitAddress)
        {
            var existingUnitAddress = await _applicationDbContext.UnitAddress.FirstOrDefaultAsync(u => u.UnitId == Id);
            if (existingUnitAddress != null)
            {
                existingUnitAddress.CountryId = unitAddress.CountryId;
                existingUnitAddress.StateId = unitAddress.StateId;
                existingUnitAddress.CityId = unitAddress.CityId;
                existingUnitAddress.AddressLine1 = unitAddress.AddressLine1;
                existingUnitAddress.AddressLine2 = unitAddress.AddressLine2;
                existingUnitAddress.PinCode = unitAddress.PinCode;
                existingUnitAddress.ContactNumber = unitAddress.ContactNumber;
                existingUnitAddress.AlternateNumber = unitAddress.AlternateNumber;
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateUnitContactsAsync(int Id, UnitContacts unitContacts)
        {
             var existingUnitContacts = await _applicationDbContext.UnitContacts.FirstOrDefaultAsync(u => u.UnitId == Id);
            if (existingUnitContacts != null)
            {
                existingUnitContacts.Name = unitContacts.Name;
                existingUnitContacts.Designation = unitContacts.Designation;
                existingUnitContacts.Email = unitContacts.Email;
                existingUnitContacts.PhoneNo = unitContacts.PhoneNo;
                existingUnitContacts.Remarks = unitContacts.Remarks;
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateUnitAsync(int Id, Unit unit)
        {
       var existingUnit = await _applicationDbContext.Unit.FirstOrDefaultAsync(u => u.Id == Id);
        if (existingUnit != null)
        {
        existingUnit.UnitName = unit.UnitName;
        existingUnit.ShortName = unit.ShortName;
        existingUnit.CompanyId = unit.CompanyId;
        existingUnit.DivisionId = unit.DivisionId;
        existingUnit.UnitHeadName = unit.UnitHeadName;
        existingUnit.CINNO = unit.CINNO;
        existingUnit.IsActive = unit.IsActive;
        await _applicationDbContext.SaveChangesAsync();
        }
        }

        public async Task DeleteUnitAsync(int Id, Unit unit)
        {
            var unitToDelete = await _applicationDbContext.Unit.FirstOrDefaultAsync(u => u.Id == Id);
            if (unitToDelete != null)
            {
                unitToDelete.IsActive = unit.IsActive;
                await _applicationDbContext.SaveChangesAsync();
               
            }
          
        
           
        }


        
    }
}