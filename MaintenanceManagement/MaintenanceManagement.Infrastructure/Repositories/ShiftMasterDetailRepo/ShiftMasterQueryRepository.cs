using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IShiftMasterDetail;
using Core.Domain.Entities;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.ShiftMasterDetailRepo
{
    public class ShiftMasterQueryRepository : IShiftMasterDetailQuery
    {
        private readonly IDbConnection _dbConnection;
        public ShiftMasterQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> AlreadyExistsAsync(int ShiftMasterId)
        {
             var query = "SELECT COUNT(1) FROM [Maintenance].[ShiftMasterDetails] WHERE ShiftMasterId = @ShiftMasterId AND IsDeleted = 0";
               
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { ShiftMasterId = ShiftMasterId });
                return count > 0;
        }

        public async Task<(List<Core.Domain.Entities.ShiftMaster>, int)> GetAllShiftMasterDetailAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
              var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM [Maintenance].[ShiftMaster] 
              WHERE IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ShiftName LIKE @Search OR ShiftCode LIKE @Search)")}};

                SELECT 
                Id, 
                ShiftCode,
                ShiftName,
                EffectiveDate,
                IsActive
            FROM [Maintenance].[ShiftMaster] 
            WHERE 
            IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ShiftName LIKE @Search OR ShiftCode LIKE @Search )")}}
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

             var shiftmaster = await _dbConnection.QueryMultipleAsync(query, parameters);
             var shiftMasterlist = (await shiftmaster.ReadAsync<Core.Domain.Entities.ShiftMaster>()).ToList();
             int totalCount = (await shiftmaster.ReadFirstAsync<int>());

            return (shiftMasterlist, totalCount);
        }

        public async Task<ShiftMasterDetail> GetByIdAsync(int ShiftMasterId)
        {
            const string query = "SELECT * FROM [Maintenance].[ShiftMasterDetails]  WHERE ShiftMasterId = @ShiftMasterId AND IsDeleted = 0";
            return await _dbConnection.QueryFirstOrDefaultAsync<ShiftMasterDetail>(query, new { ShiftMasterId =ShiftMasterId });
        }

        public Task<List<ShiftMasterDetail>> GetShiftMasterDetail(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> NotFoundAsync(int ShiftMasterId)
        {
             var query = "SELECT COUNT(1) FROM [Maintenance].[ShiftMasterDetails] WHERE ShiftMasterId = @ShiftMasterId AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { ShiftMasterId = ShiftMasterId });
                return count > 0;
        }

        public Task<bool> SoftDeleteValidation(int Id)
        {
            throw new NotImplementedException();
        }
    }
}