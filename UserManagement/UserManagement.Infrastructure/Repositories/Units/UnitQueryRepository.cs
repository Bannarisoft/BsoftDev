using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUnit;
using System.Data;
using Dapper;
using Core.Application.Units.Queries.GetUnits;

namespace UserManagement.Infrastructure.Repositories.Units
{
    public class UnitQueryRepository : IUnitQueryRepository
    {
        private readonly IDbConnection _dbConnection;  

         

        public UnitQueryRepository(IDbConnection dbConnection)
        {
          _dbConnection = dbConnection;
        }

        public async Task<(List<Unit>, int)> GetAllUnitsAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
            DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM AppData.Unit C
              WHERE C.IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (C.UnitName LIKE @Search OR C.ShortName LIKE @Search)")}};

                SELECT 
            C.Id, 
            C.UnitName, 
            C.ShortName,
            C.CompanyId,
            C.DivisionId,
            C.UnitHeadName,
            C.CINNO,
            C.IsActive,
            C.OldUnitId
             FROM AppData.Unit C
              WHERE C.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (C.UnitName LIKE @Search OR C.ShortName LIKE @Search)")}}
              ORDER BY C.Id DESC
              OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

             SELECT @TotalCount AS TotalCount;
            """;
            
            
            var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };
              var unit = await _dbConnection.QueryMultipleAsync(query, parameters);
             var unitslits = (await unit.ReadAsync<Unit>()).ToList();
             int totalCount = (await unit.ReadFirstAsync<int>());
            return (unitslits, totalCount);
        }

        public async Task<Unit> GetByIdAsync(int Id)
        {
            const string query = @"
             SELECT 
            C.Id, 
            C.UnitName, 
            C.ShortName,
            C.CompanyId,
            C.DivisionId,
            C.UnitHeadName,
            C.CINNO,
            C.IsActive,
            C.OldUnitId,
            A.CountryId,
            A.StateId,
            A.CityId,
            A.AddressLine1,
            A.AddressLine2,
            A.PinCode,
            A.ContactNumber,
            A.AlternateNumber,
            B.Name,
                 B.Designation,
                 B.Email,
                 B.PhoneNo ,
                 B.Remarks As Remarks 
             FROM AppData.Unit C
             LEFT JOIN AppData.UnitAddress A ON A.UnitId = C.Id
             LEFT JOIN AppData.UnitContacts B ON B.UnitId = C.Id
             WHERE C.Id = @id AND C.IsDeleted = 0";
    var unitResponse = await _dbConnection.QueryAsync<Unit,UnitAddress,UnitContacts,Unit>(query, 
    (unit,unitaddress,unitcontacts) =>
    {
        unit.UnitAddress = unitaddress;
        unit.UnitContacts = unitcontacts;
        return unit;
        }, 
    new { Id },
    splitOn: "CountryId,Name");

             return unitResponse.FirstOrDefault();
        }

        public async Task<List<Unit>> GetUnit(string searchPattern, int userId,int CompanyId)
        {
             const string query = @"
                SELECT 
                U.Id, 
                U.UnitName,
                U.DivisionId
            FROM AppData.Unit U
            Inner join [AppSecurity].[UserUnit] UU on UU.UnitId = U.Id where IsDeleted = 0 
            and UnitName like @SearchPattern and UU.UserId = @UserId and UU.IsActive = 1 and U.CompanyId = @CompanyId";
                
              var result = await _dbConnection.QueryAsync<Unit>(query, new 
            { 
                SearchPattern = $"%{searchPattern}%",
                 UserId = userId,
                 CompanyId = CompanyId
            
             });
            return result.ToList();
        }
         public async Task<List<Unit>> GetUnitByUserId(int userId,int CompanyId)
        {
             const string query = @"
                SELECT 
                U.Id, 
                U.UnitName,
                U.DivisionId
            FROM AppData.Unit U
            Inner join [AppSecurity].[UserUnit] UU on UU.UnitId = U.Id where IsDeleted = 0 
            and UU.UserId = @UserId and UU.IsActive = 1 and U.CompanyId = @CompanyId";
                
              var result = await _dbConnection.QueryAsync<Unit>(query, new 
            {                
                 UserId = userId,
                 CompanyId = CompanyId
            
             });
            return result.ToList();
        }
         public async Task<bool> FKColumnExistValidation(int Id)
          {
              var sql = "SELECT COUNT(1) FROM AppData.Unit WHERE Id = @Id AND IsDeleted = 0 AND IsActive = 1";
                var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Id = Id });
                return count > 0;
          }
            public async Task<List<Unit>> GetUnit_SuperAdmin(string searchPattern)
        {
             const string query = @"
                SELECT 
                U.Id, 
                U.UnitName,
                U.DivisionId
            FROM AppData.Unit U
            Inner join [AppSecurity].[UserUnit] UU on UU.UnitId = U.Id where IsDeleted = 0 
            and UnitName like @SearchPattern";
                
              var result = await _dbConnection.QueryAsync<Unit>(query, new 
            { 
                SearchPattern = $"%{searchPattern}%"
            
             });
            return result.ToList();
        }
  

                  

    }

    }
