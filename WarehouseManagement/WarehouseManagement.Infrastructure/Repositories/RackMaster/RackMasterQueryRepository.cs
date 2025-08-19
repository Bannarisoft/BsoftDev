using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IRackMaster;
using Core.Application.RackMaster.Queries.GetAllRackMaster;
using Core.Application.RackMaster.Queries.GetRackMasterAutoComplete;
using Dapper;

namespace WarehouseManagement.Infrastructure.Repositories.RackMaster
{
    public class RackMasterQueryRepository : IRackMasterQueryRepository
    {

        private readonly IDbConnection _dbConnection;

        public RackMasterQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        public async Task<(List<RackMasterDto>, int)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm)
        {
            var sql = $@"
                        DECLARE @TotalCount INT;

                        SELECT @TotalCount = COUNT(*)
                        FROM Warehouse.RackMaster r WITH (NOLOCK)
                        WHERE r.IsDeleted = 0
                        {(string.IsNullOrWhiteSpace(searchTerm) ? "" : "AND (r.RackCode LIKE @Search OR r.RackName LIKE @Search)")};

                        SELECT 
                            r.Id,
                            r.WarehouseId,
                            r.RackCode,
                            r.RackName,
                            r.FloorId,
                            r.AisleId,
                            r.RackLevelId,
                            r.MaxCapacity,
                            r.CapacityUOMId,
                            r.RackWidth,
                            r.RackHeight,
                            r.DimensionUOMId,
                            r.IsActive,
                            r.IsDeleted,
                            r.CreatedBy,
                            r.CreatedDate,
                            r.CreatedByName,
                            r.CreatedIP,
                            r.ModifiedBy,
                            r.ModifiedDate,
                            r.ModifiedByName,
                            r.ModifiedIP
                        FROM Warehouse.RackMaster r WITH (NOLOCK)
                        WHERE r.IsDeleted = 0
                        {(string.IsNullOrWhiteSpace(searchTerm) ? "" : "AND (r.RackCode LIKE @Search OR r.RackName LIKE @Search)")}
                        ORDER BY r.Id DESC
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        SELECT @TotalCount AS TotalCount;";

            var parameters = new
            {
                Search = string.IsNullOrWhiteSpace(searchTerm) ? null : $"%{searchTerm.Trim()}%",
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };

            using var multi = await _dbConnection.QueryMultipleAsync(sql, parameters);
            var items = (await multi.ReadAsync<RackMasterDto>()).ToList();
            var total = await multi.ReadFirstAsync<int>();
            return (items, total);
        }

        public async Task<RackMasterDto?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    r.Id,
                    r.WarehouseId,
                    r.RackCode,
                    r.RackName,
                    r.FloorId,
                    r.AisleId,
                    r.RackLevelId,
                    r.MaxCapacity,
                    r.CapacityUOMId,
                    r.RackWidth,
                    r.RackHeight,
                    r.DimensionUOMId,
                    r.IsActive,
                    r.IsDeleted,
                    r.CreatedBy,
                    r.CreatedDate,
                    r.CreatedByName,
                    r.CreatedIP,
                    r.ModifiedBy,
                    r.ModifiedDate,
                    r.ModifiedByName,
                    r.ModifiedIP
                FROM Warehouse.RackMaster r
                WHERE r.Id = @Id AND r.IsDeleted = 0;";

            return await _dbConnection.QueryFirstOrDefaultAsync<RackMasterDto>(sql, new { Id = id });
        }

        public async Task<bool> RackSlotAlreadyExistsAsync(int warehouseId, int? floorId, int? aisleId, int? rackLevelId, int? id = null)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM Warehouse.RackMaster WITH (NOLOCK)
                WHERE IsDeleted = 0
                AND WarehouseId = @WarehouseId
                AND (
                        (FloorId = @FloorId) OR (FloorId IS NULL AND @FloorId IS NULL)
                    )
                AND (
                        (AisleId = @AisleId) OR (AisleId IS NULL AND @AisleId IS NULL)
                    )
                AND (
                        (RackLevelId = @RackLevelId) OR (RackLevelId IS NULL AND @RackLevelId IS NULL)
                    )
                AND (@Id IS NULL OR Id <> @Id);";

            var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new
            {
                WarehouseId = warehouseId,
                FloorId = floorId,
                AisleId = aisleId,
                RackLevelId = rackLevelId,
                Id = id
            });

            return count > 0;
        }

           public async Task<List<GetRackMasterAutoCompleteDto>> GetRackMasterAutoCompletes(string searchPattern)
                {
                    const string sql = @"
                    SELECT 
                        r.Id,
                        r.RackCode,
                        r.RackName
                    FROM   [Warehouse].[Warehouse].[RackMaster] r
                    WHERE  r.IsDeleted = 0
                    AND (@Term = '' 
                        OR r.RackCode LIKE '%' + @Term + '%'
                        OR (r.RackName IS NOT NULL AND r.RackName LIKE '%' + @Term + '%'))
                    ORDER BY r.RackCode, r.RackName;";

                    var rows = await _dbConnection.QueryAsync<GetRackMasterAutoCompleteDto>(sql, new
                    {
                        Term = (searchPattern ?? string.Empty).Trim(),
                      
                    });

                    return rows.ToList();
                }
        
        

    }
}