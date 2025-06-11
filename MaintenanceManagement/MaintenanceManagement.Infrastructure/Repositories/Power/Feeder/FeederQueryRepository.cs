using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.Power.IFeeder;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.Power.Feeder
{
    public class FeederQueryRepository : IFeederQueryRepository
    {

        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
        public FeederQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }

        public async Task<(List<Core.Domain.Entities.Power.Feeder>, int)> GetAllFeederAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var query = $$"""
            DECLARE @TotalCount INT;
            SELECT @TotalCount = COUNT(*) 
            FROM [Maintenance].[Feeder] F
            WHERE F.IsDeleted = 0
            {{(string.IsNullOrEmpty(searchTerm) ? "" : "AND (F.FeederName LIKE @Search OR F.FeederCode LIKE @Search)")}}; 

            SELECT 
                F.Id,
                F.FeederCode,
                F.FeederName,
                F.ParentFeederId,
                F.FeederGroupId,
                F.FeederTypeId,
                F.DepartmentId,
                F.Description,
                F.MultiplicationFactor,
                F.EffectiveDate,
                F.OpeningReading,
                F.HighPriority,
                F.IsActive,
                F.IsDeleted,
                F.CreatedBy,
                F.CreatedDate,
                F.CreatedByName,
                F.CreatedIP,
                F.ModifiedBy,
                F.ModifiedDate,
                F.ModifiedByName,
                F.ModifiedIP,
                F.UnitId
            FROM [Maintenance].[Feeder] F
            WHERE F.IsDeleted = 0 AND F.UnitId = @UnitId
            {{(string.IsNullOrEmpty(searchTerm) ? "" : "AND (F.FeederName LIKE @Search OR F.FeederCode LIKE @Search)")}}
            ORDER BY F.Id DESC 
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT @TotalCount AS TotalCount;
            """;

            var parameters = new
            {
                Search = $"%{searchTerm}%",
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize,
                UnitId

            };

            var result = await _dbConnection.QueryMultipleAsync(query, parameters);

            var feederList = (await result.ReadAsync<Core.Domain.Entities.Power.Feeder>()).ToList();
            int totalCount = await result.ReadFirstAsync<int>();

            return (feederList, totalCount);
        }

        public async Task<Core.Domain.Entities.Power.Feeder> GetFeederByIdAsync(int id)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var query = """
                SELECT 
                    Id,FeederCode,FeederName,ParentFeederId,FeederGroupId,FeederTypeId,DepartmentId,Description,MultiplicationFactor,EffectiveDate,OpeningReading,HighPriority,IsActive,
                    IsDeleted,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP ,UnitId FROM [Maintenance].[Feeder]
                WHERE IsDeleted = 0 AND Id = @Id AND UnitId = @UnitId;
            """;

            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.Power.Feeder>(query, new { Id = id , UnitId});
            return result.FirstOrDefault();
        }

        public async Task<bool> AlreadyExistsAsync(string feederCode, int? id = null)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var query = "SELECT COUNT(1) FROM [Maintenance].[Feeder] WHERE FeederCode = @feederCode AND IsDeleted = 0 AND UnitId = @UnitId";

            var parameters = new DynamicParameters();
            parameters.Add("FeederCode", feederCode);
            parameters.Add("UnitId", UnitId);

            if (id is not null)
            {
                query += " AND Id != @Id";
                parameters.Add("Id", id);
            }
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
            return count > 0;
        }

        public async Task<bool> NotFoundAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM Maintenance.Feeder WHERE Id = @Id AND IsDeleted = 0";

            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
            return count > 0;
        }

        public async Task<List<Core.Domain.Entities.Power.Feeder>> GetFeederAutoComplete(string searchPattern)
        {
            var UnitId = _ipAddressService.GetUnitId();
            const string query = @"
                       SELECT Id, FeederCode,FeederName  
                       FROM Maintenance.Feeder
                       WHERE  ParentFeederId is null AND  IsDeleted = 0 AND (FeederName LIKE @SearchPattern OR FeederCode LIKE @SearchPattern) AND UnitId = @UnitId";
            var parameters = new
            {
                SearchPattern = $"%{searchPattern ?? string.Empty}%",
                UnitId
            };
            var feeder = await _dbConnection.QueryAsync<Core.Domain.Entities.Power.Feeder>(query, parameters);

            return feeder.ToList();
        } 

       

        
 


    }
}