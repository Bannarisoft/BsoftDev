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

        public async Task<List<Unit>> GetAllUnitsAsync()
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
                ORDER BY 
                    u.CreatedAt DESC
            ";

             var unitDictionary = new Dictionary<int, Unit>();
            
            var units = await _dbConnection.QueryAsync<Unit, UnitAddress, UnitContacts, Unit>(
                query,
                (unit, address, contact) =>
                {
                    if (!unitDictionary.TryGetValue(unit.Id, out var existingunit))
                    {
                        existingunit = unit;
                        existingunit.UnitAddress = address;
                        existingunit.UnitContacts = contact;
                        unitDictionary.Add(existingunit.Id, existingunit);
                    }
                    else
                    {
                        existingunit.UnitAddress = address;
                        existingunit.UnitContacts = contact;
                    }
            
                    return existingunit;
                },
                splitOn: "AddressLine1,Name" 
            );
            
            return units.ToList();

        }

        public async Task<List<Unit>> GetByIdAsync(int id)
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
                WHERE u.Id = @id
                ORDER BY 
                    u.CreatedAt DESC
            ";

            var unitDictionary = new Dictionary<int, Unit>();
            
            var units = await _dbConnection.QueryAsync<Unit, UnitAddress, UnitContacts, Unit>(
                query,
                (unit, address, contact) =>
                {
                    if (!unitDictionary.TryGetValue(unit.Id, out var existingunit))
                    {
                        existingunit = unit;
                        existingunit.UnitAddress = address;
                        existingunit.UnitContacts = contact;
                        unitDictionary.Add(existingunit.Id, existingunit);
                    }
                    else
                    {
                        existingunit.UnitAddress = address;
                        existingunit.UnitContacts = contact;
                    }
            
                    return existingunit;
                },
                splitOn: "AddressLine1,Name",
                param: new { id = id }
            );
            
            return units.ToList();
        }


         public async Task<List<Unit>> GetUnit(string searchPattern = null)
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
                ORDER BY 
                    u.CreatedAt DESC";

            var unitDictionary = new Dictionary<int, Unit>();
            
            var units = await _dbConnection.QueryAsync<Unit, UnitAddress, UnitContacts, Unit>(
                query,
                (unit, address, contact) =>
                {
                    if (!unitDictionary.TryGetValue(unit.Id, out var existingunit))
                    {
                        existingunit = unit;
                        existingunit.UnitAddress = address;
                        existingunit.UnitContacts = contact;
                        unitDictionary.Add(existingunit.Id, existingunit);
                    }
                    else
                    {
                        existingunit.UnitAddress = address;
                        existingunit.UnitContacts = contact;
                    }
            
                    return existingunit;
                },
                splitOn: "AddressLine1,Name",
                param: new { SearchPattern = $"%{searchPattern}%" }
            );
            
            return units.ToList();
        }             
            
        }

    }
