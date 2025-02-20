using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Domain.Common;
using Core.Domain.Entities;
using Dapper;

namespace FAM.Infrastructure.Repositories.Manufacture
{
    public class ManufactureQueryRepository : IManufactureQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public ManufactureQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }     
        public async Task<(List<Manufactures>, int)> GetAllManufactureAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM FixedAsset.Manufacture 
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Code LIKE @Search OR ManufactureName LIKE @Search)")}};

                SELECT Id,Code,ManufactureName,ManufactureType,CountryId,StateId,CityId,AddressLine1,AddressLine2,PinCode,PersonName,PhoneNumber,Email,  IsActive
                ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
                FROM FixedAsset.Manufacture  WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Code LIKE @Search OR ManufactureName LIKE @Search )")}}
                ORDER BY Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                SELECT @TotalCount AS TotalCount;
                """;
            var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };

            var manufacture = await _dbConnection.QueryMultipleAsync(query, parameters);
            var manufactureList = (await manufacture.ReadAsync<Manufactures>()).ToList();
            int totalCount = (await manufacture.ReadFirstAsync<int>());             
            return (manufactureList, totalCount);             
        }

        public async Task<List<Manufactures>> GetByManufactureNameAsync(string searchPattern)
        {
            const string query = @"
            SELECT Id,Code,ManufactureName,ManufactureType,CountryId,StateId,CityId,AddressLine1,AddressLine2,PinCode,PersonName,PhoneNumber,Email,  IsActive
                ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
            FROM FixedAsset.Manufacture 
            WHERE (ManufactureName LIKE @SearchPattern OR Code LIKE @SearchPattern) 
            AND  IsDeleted=0 and IsActive=1
            ORDER BY ID DESC";            
            var result = await _dbConnection.QueryAsync<Manufactures>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }

        public async Task<Manufactures> GetByIdAsync(int Id)
        {
            const string query = @"
            SELECT Id,Code,ManufactureName,ManufactureType,CountryId,StateId,CityId,AddressLine1,AddressLine2,PinCode,PersonName,PhoneNumber,Email,  IsActive
                ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
            FROM FixedAsset.Manufacture WHERE Id = @Id AND IsDeleted=0";
            var manufacture = await _dbConnection.QueryFirstOrDefaultAsync<Manufactures>(query, new { Id });           
            if (manufacture is null)
            {
                throw new KeyNotFoundException($"Manufacture with ID {Id} not found.");
            }
            return manufacture;
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetManufactureTypeAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";          
            var parameters = new { MiscTypeCode = MiscEnumEntity.Manufacture_ManufactureType.MiscCode };     
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
    }
}