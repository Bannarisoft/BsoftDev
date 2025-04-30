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
    public class ShiftMasterDetailQueryRepository : IShiftMasterDetailQuery
    {
        private readonly IDbConnection _dbConnection;
        public ShiftMasterDetailQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> AlreadyExistsAsync(int ShiftMasterId)
        {
             var query = "SELECT COUNT(1) FROM [Maintenance].[ShiftMasterDetails] WHERE ShiftMasterId = @ShiftMasterId AND IsDeleted = 0";
               
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { ShiftMasterId = ShiftMasterId });
                return count > 0;
        }

        public async Task<bool> FKColumnValidation(int ShiftMasterId)
        {
            var query = "SELECT COUNT(1) FROM [Maintenance].[ShiftMaster] WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = ShiftMasterId });
                return count > 0;
        }

        public async Task<(IEnumerable<dynamic>, int)> GetAllShiftMasterDetailAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
              var query = $$"""
             DECLARE @TotalCount INT;
             SELECT @TotalCount = COUNT(*) 
               FROM [Maintenance].[ShiftMaster] SM
            INNER JOIN [Maintenance].[ShiftMasterDetails] SMD ON SMD.ShiftMasterId=SM.Id
              WHERE SMD.IsDeleted = 0
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (SM.ShiftName LIKE @Search OR SM.ShiftCode LIKE @Search)")}};

                SELECT 
                SMD.Id,
                SM.ShiftCode,
                SM.ShiftName,
                SMD.StartTime,
                SMD.EndTime,
                SMD.DurationInHours,
                SMD.BreakDurationInMinutes,
                 Cast(SMD.EffectiveDate as varchar) AS EffectiveDate
            FROM [Maintenance].[ShiftMaster] SM
            INNER JOIN [Maintenance].[ShiftMasterDetails] SMD ON SMD.ShiftMasterId=SM.Id
            WHERE 
            SMD.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (SM.ShiftName LIKE @Search OR SM.ShiftCode LIKE @Search )")}}
                ORDER BY SMD.Id desc
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
             var shiftMasterlist = await shiftmaster.ReadAsync<dynamic>();
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

        public async Task<bool> NotFoundAsync(int Id)
        {
             var query = "SELECT COUNT(1) FROM [Maintenance].[ShiftMasterDetails] WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = Id });
                return count > 0;
        }

        public Task<bool> SoftDeleteValidation(int Id)
        {
            throw new NotImplementedException();
        }
    }
}