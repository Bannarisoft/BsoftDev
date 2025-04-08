using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;

namespace MaintenanceManagement.Infrastructure.Repositories.PreventiveSchedulers
{
    public class PreventiveSchedulerQueryRepository : IPreventiveSchedulerQuery
    {
        private readonly IDbConnection _dbConnection;
        public PreventiveSchedulerQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public Task<bool> AlreadyExistsAsync(string ShiftName, int? id = null)
        {
            throw new NotImplementedException();
        }

        public async Task<(List<PreventiveSchedulerHdr>, int)> GetAllPreventiveSchedulerAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            //  var query = $$"""
            //  DECLARE @TotalCount INT;
            //  SELECT @TotalCount = COUNT(*) 
            //    FROM [Maintenance].[ShiftMaster] 
            //   WHERE IsDeleted = 0
            // {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ShiftName LIKE @Search OR ShiftCode LIKE @Search)")}};

            //     SELECT 
            //     Id, 
            //     ShiftCode,
            //     ShiftName,
            //     EffectiveDate,
            //     IsActive
            // FROM [Maintenance].[ShiftMaster] 
            // WHERE 
            // IsDeleted = 0
            //     {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (ShiftName LIKE @Search OR ShiftCode LIKE @Search )")}}
            //     ORDER BY Id desc
            //     OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            //     SELECT @TotalCount AS TotalCount;
            // """;

            
            //  var parameters = new
            //            {
            //                Search = $"%{SearchTerm}%",
            //                Offset = (PageNumber - 1) * PageSize,
            //                PageSize
            //            };

            //  var shiftmaster = await _dbConnection.QueryMultipleAsync(query, parameters);
            //  var shiftMasterlist = (await shiftmaster.ReadAsync<Core.Domain.Entities.ShiftMaster>()).ToList();
            //  int totalCount = (await shiftmaster.ReadFirstAsync<int>());

            // return (shiftMasterlist, totalCount);
            throw new NotImplementedException();
        }

        public Task<PreventiveSchedulerHdr> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PreventiveSchedulerHdr>> GetPreventiveScheduler(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public Task<bool> NotFoundAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SoftDeleteValidation(int Id)
        {
            throw new NotImplementedException();
        }
    }
}