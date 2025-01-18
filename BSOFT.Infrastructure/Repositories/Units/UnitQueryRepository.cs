using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUnit;
using System.Data;
using Dapper;
using Core.Application.Units.Queries.GetUnits;

namespace BSOFT.Infrastructure.Repositories.Units
{
    public class UnitQueryRepository : IUnitQueryRepository
    {
        private readonly IDbConnection _dbConnection;        

        public UnitQueryRepository(IDbConnection dbConnection)
        {
          _dbConnection = dbConnection;
        }

        public async Task<List<UnitDto>> GetAllUnitsAsync()
        {
            var query = @"
                SELECT 
                    u.Id,
                    u.UnitName,
                    u.ShortName,
                    u.CompanyId,
                    u.DivisionId,
                    u.UnitHeadName,
                    u.CINNO,
                    u.IsActive,
                    ua.Id as AddressId,
                    ua.UnitId,
                    ua.CountryId,
                    ua.StateId,
                    ua.CityId,
                    ua.AddressLine1,
                    ua.AddressLine2,
                    ua.PinCode,
                    ua.ContactNumber,
                    ua.AlternateNumber,
                    uc.Id as ContactId,
                    uc.UnitId,
                    uc.Name,
                    uc.Designation,
                    uc.Email,
                    uc.PhoneNo,
                    uc.Remarks                   
                FROM 
                    AppData.Unit  u
                INNER JOIN 
                    AppData.UnitAddress  ua ON u.Id = ua.UnitId
                INNER JOIN 
                    AppData.UnitContacts uc ON u.Id = uc.UnitId
            ";

            var result = await _dbConnection.QueryAsync<UnitDto, UnitAddressDto, UnitContactsDto, UnitDto>(
            query,
            (unit, address, contact) =>
            {
                unit.UnitAddressDto.Add(address);
                unit.UnitContactsDto.Add(contact);
                return unit;
            },
            splitOn: "AddressId, ContactId");
            var units = result.GroupBy(u => u.Id)
            .Select(g => g.First())
            .ToList(); 
            return units;
        }

        public async Task<List<UnitDto>> GetByIdAsync(int id)
        {
                var query = @"
                SELECT 
                    u.Id,
                    u.UnitName,
                    u.ShortName,
                    u.CompanyId,
                    u.DivisionId,
                    u.UnitHeadName,
                    u.CINNO,
                    u.IsActive,
                    ua.Id as AddressId,
                    ua.UnitId,
                    ua.CountryId,
                    ua.StateId,
                    ua.CityId,
                    ua.AddressLine1,
                    ua.AddressLine2,
                    ua.PinCode,
                    ua.ContactNumber,
                    ua.AlternateNumber,
                    uc.Id as ContactId,
                    uc.UnitId,
                    uc.Name,
                    uc.Designation,
                    uc.Email,
                    uc.PhoneNo,
                    uc.Remarks                   
                FROM 
                    AppData.Unit  u
                INNER JOIN 
                    AppData.UnitAddress  ua ON u.Id = ua.UnitId
                INNER JOIN 
                    AppData.UnitContacts uc ON u.Id = uc.UnitId
            ";

            var result = await _dbConnection.QueryAsync<UnitDto, UnitAddressDto, UnitContactsDto, UnitDto>(
            query,
            (unit, address, contact) =>
            {
            unit.UnitAddressDto.Add(address);
            unit.UnitContactsDto.Add(contact);
            return unit;
            },
            splitOn: "AddressId, ContactId");
            var units = result.Where(u => u.Id == id)
            .GroupBy(u => u.Id)
            .Select(g => g.First())
            .ToList();
            return units; 
        }


         public async Task<List<UnitDto>> GetUnit(string searchPattern = null)
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                throw new ArgumentException("Unitname cannot be null or empty.", nameof(searchPattern));
            }

            const string query = @"
                SELECT 
                    u.Id,
                    u.UnitName,
                    u.ShortName,
                    u.CompanyId,
                    u.DivisionId,
                    u.UnitHeadName,
                    u.CINNO,
                    u.IsActive,
                    ua.Id as AddressId,
                    ua.UnitId,
                    ua.CountryId,
                    ua.StateId,
                    ua.CityId,
                    ua.AddressLine1,
                    ua.AddressLine2,
                    ua.PinCode,
                    ua.ContactNumber,
                    ua.AlternateNumber,
                    uc.Id as ContactId,
                    uc.UnitId,
                    uc.Name,
                    uc.Designation,
                    uc.Email,
                    uc.PhoneNo,
                    uc.Remarks                   
                FROM 
                    AppData.Unit  u
                INNER JOIN 
                    AppData.UnitAddress  ua ON u.Id = ua.UnitId
                INNER JOIN 
                    AppData.UnitContacts uc ON u.Id = uc.UnitId
				WHERE U.UnitName LIKE @SearchPattern OR U.Id LIKE @SearchPattern
                AND U.IsActive = 1
                ORDER BY U.UnitName";
                var result = await _dbConnection.QueryAsync<UnitDto, UnitAddressDto, UnitContactsDto, UnitDto>(
                query,
                (unit, address, contact) =>
                {
                    unit.UnitAddressDto.Add(address);
                    unit.UnitContactsDto.Add(contact);
                    return unit;
                },
                splitOn: "AddressId, ContactId",
                //param: new { SearchPattern = $"%{SearchPattern}%" });
                param: new { SearchPattern = $"%{searchPattern}%" });
                var units = result.GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList(); 
                return units;                  
            
        }

    }
}