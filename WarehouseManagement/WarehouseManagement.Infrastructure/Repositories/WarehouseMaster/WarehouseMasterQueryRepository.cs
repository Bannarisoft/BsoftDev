using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IWarehouseMaster;
using Core.Application.WarehouseMaster.GetAllWarehouseMaster;
using Dapper;

namespace WarehouseManagement.Infrastructure.Repositories.WarehouseMaster
{
    public class WarehouseMasterQueryRepository : IWarehouseMasterQueryRepository
    {


        private readonly IDbConnection _dbConnection;

        public WarehouseMasterQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<(List<WarehouseMasterDto>, int)> GetAllAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = $@"
                DECLARE @TotalCount INT;

                SELECT @TotalCount = COUNT(*)
                FROM Warehouse.WarehouseMaster w
                WHERE w.IsDeleted = 0
                {(string.IsNullOrWhiteSpace(searchTerm) ? "" : "AND (w.WarehouseCode LIKE @Search OR w.WarehouseName LIKE @Search OR w.ContactPersonName LIKE @Search)")};

                SELECT 
                    w.Id,
                    w.WarehouseCode,
                    w.WarehouseName,
                    w.UnitId,
                    w.ParentWarehouseId,
                    w.IsGroup,
                    w.WarehouseTypeId,
                    w.StorageTypeId,
                    w.CapacityUOMId,
                    w.AccountId,
                    w.ContactPersonName,
                    w.MobileNumber,
                    w.Email,
                    w.AddressLine1,
                    w.AddressLine2,
                    w.CityId,
                    w.StateId,
                    w.CountryId,
                    w.Pincode,
                    w.IsScrapWarehouse,
                    w.IsTransitWarehouse,
                    w.MaxCapacity,
                    w.IsDefaultStockEntry,
                    w.IsActive,
                    w.IsDeleted,
                    w.CreatedBy,
                    w.CreatedDate,
                    w.CreatedByName,
                    w.CreatedIP,
                    w.ModifiedBy,
                    w.ModifiedDate,
                    w.ModifiedByName,
                    w.ModifiedIP
                FROM Warehouse.WarehouseMaster w
                WHERE w.IsDeleted = 0
                {(string.IsNullOrWhiteSpace(searchTerm) ? "" : "AND (w.WarehouseCode LIKE @Search OR w.WarehouseName LIKE @Search OR w.ContactPersonName LIKE @Search)")}
                ORDER BY w.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            ";

            var parameters = new
            {
                Search = $"%{searchTerm}%",
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };

            using var multi = await _dbConnection.QueryMultipleAsync(query, parameters);
            var warehouseList = (await multi.ReadAsync<WarehouseMasterDto>()).ToList();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (warehouseList, totalCount);
        }
        
         public async Task<WarehouseMasterDto?> GetByIdAsync(int id)
            {
                var query = @"
                    SELECT 
                        wm.Id, wm.WarehouseCode, wm.WarehouseName, wm.UnitId,
                        wm.ParentWarehouseId, wm.IsGroup, wm.WarehouseTypeId,
                        wm.StorageTypeId, wm.CapacityUOMId, wm.AccountId,
                        wm.ContactPersonName, wm.MobileNumber, wm.Email,
                        wm.AddressLine1, wm.AddressLine2, wm.CityId, wm.StateId,
                        wm.CountryId, wm.Pincode, wm.IsScrapWarehouse, wm.IsTransitWarehouse,
                        wm.MaxCapacity, wm.IsDefaultStockEntry, wm.IsActive, wm.IsDeleted,
                        wm.CreatedBy, wm.CreatedDate, wm.CreatedByName, wm.CreatedIP,
                        wm.ModifiedBy, wm.ModifiedDate, wm.ModifiedByName, wm.ModifiedIP
                    FROM Warehouse.WarehouseMaster wm
                    WHERE wm.Id = @Id AND wm.IsDeleted = 0";

                return await _dbConnection.QueryFirstOrDefaultAsync<WarehouseMasterDto>(query, new { Id = id });
            }
        
    }
}