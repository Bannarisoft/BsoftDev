using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Application.PartyMaster.Queries.GetPartyGroupLoad;
using Dapper;

namespace PartyManagement.Infrastructure.Repositories.PartyMaster
{
    public class PartyMasterQueryRepository : IPartyMasterQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public PartyMasterQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<List<PartyGroupLoadDto>> GetPartyGroupsAsync(List<int> groupTypeIds)
        {
            var query = @"
                SELECT 
                    pg.Id GroupId, 
                    ISNULL(ppg.PartyGroupName, '') + '-' + pg.PartyGroupName AS PartyGroupName,
                    mm.Id as PartyTypeId,
	                mm.description as PartyTypeName
                FROM Party.PartyGroup pg
                LEFT JOIN Party.PartyGroup ppg 
                    ON pg.ParentPartyGroupId = ppg.Id
                INNER JOIN Party.MiscMaster mm
                    ON pg.GroupTypeId = mm.Id
                WHERE pg.IsDeleted = 0 
                  AND pg.IsGroup = 0 
                  AND pg.IsActive = 1
                  AND mm.Id IN @GroupTypeIds";

            var result = await _dbConnection.QueryAsync<PartyGroupLoadDto>(query, new { GroupTypeIds = groupTypeIds });
            return result.ToList();
        }

    }
}