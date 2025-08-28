using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces.IHangfire;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.HangFire
{
    public class HangfireQueryRepository : IHangfireQuery
    {
        private readonly IDbConnection _dbConnection;
        public HangfireQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<List<int>> GetHangfireJobByTransactionId(int arg)
        {
            const string sql = @"
            SELECT j.Id
            FROM HangFire.Job j
            LEFT JOIN HangFire.[State] s ON s.JobId = j.Id AND s.Id = j.StateId
            WHERE (
                    JSON_VALUE(j.Arguments, '$[0]') = @arg
                 OR JSON_VALUE(j.Arguments, '$[0]') = TRY_CONVERT(int, @arg)
                  )
              AND s.Name IN ('Scheduled','Enqueued','Processing');";

            var ids = (await _dbConnection.QueryAsync<int>(sql, new { arg })).ToList();
            // foreach (var id in ids)
            //     BackgroundJob.Delete(id.ToString());

            return ids.ToList();
        }
    }
}