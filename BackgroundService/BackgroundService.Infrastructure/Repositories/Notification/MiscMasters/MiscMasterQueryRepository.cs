using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.Interfaces.IMiscMaster;
using BackgroundService.Domain.Entities.Notification;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Notification.MiscMasters
{
    public class MiscMasterQueryRepository : IMiscMasterQuery
    {
        private readonly IDbConnection _dbConnection;
        public MiscMasterQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<MiscMaster> GetMiscMasterByName(string miscTypeCode, string miscTypeName)
        {
            const string query = @"SELECT M.Id,M.Code ,M.Description  FROM Maintenance.MiscMaster AS M
                                INNER JOIN Maintenance.MiscTypeMaster AS MT 
                                ON MT.Id = M.MiscTypeId
                                WHERE M.IsDeleted = 0 AND MT.IsDeleted = 0 AND M.IsActive = 1 AND MT.MiscTypeCode= @MiscTypeCode AND M.Code=@MiscTypeName  ";


            var parameters = new
            {
                MiscTypeName = miscTypeName,
                MiscTypeCode = miscTypeCode

            };

            var miscmaster = await _dbConnection.QueryFirstOrDefaultAsync<MiscMaster>(query, parameters);
            return miscmaster;
        }
    }
}