using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Application.PartyMaster.Queries.GetPartMaster;
using Core.Application.PartyMaster.Queries.GetPartMasterAutoComplete;
using Core.Application.PartyMaster.Queries.GetPartyGroupLoad;
using Core.Application.PartyMaster.Queries.GetPartyMasterById;
using Core.Domain.Common;
using Core.Domain.Entities;
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

        public async Task<string> GetDocumentDirectoryAsync()
        {
            const string query = @"
            SELECT Description            
            FROM Party.MiscTypeMaster             
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  IsDeleted=0 and IsActive=1
            ORDER BY ID DESC";
            var parameters = new { MiscTypeCode = MiscEnumEntity.PartyDocumentImage.MiscCode };
            var result = await _dbConnection.QueryAsync<string>(query, parameters);
            return result.FirstOrDefault();
        }
         public async Task<string> GetBaseDirectoryAsync()
        {
            var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "dbo.Party_GetBaseDirectory", 
                commandType: CommandType.StoredProcedure);
            return result ?? string.Empty; // return an empty string if result is null
        }

        public async Task<PartyMasterDto> GetByIdPartyMasterAsync(int id)
        {
            var sql = @"
            SELECT * FROM Party.PartyMaster WHERE Id = @Id;
            SELECT * FROM Party.PartyType WHERE PartyId = @Id;
            SELECT * FROM Party.PartyContact WHERE PartyId = @Id;
            SELECT * FROM Party.PartyAddress WHERE PartyId = @Id;
            SELECT * FROM Party.PartyBank WHERE PartyId = @Id;
            SELECT * FROM Party.PartyDocument WHERE PartyId = @Id;
        ";

            using var multi = await _dbConnection.QueryMultipleAsync(sql, new { Id = id });
            var partyMaster = await multi.ReadFirstOrDefaultAsync<PartyMasterDto>();

            if (partyMaster is null)
                return null; // Let handler handle NotFound

            partyMaster.PartyTypes = (await multi.ReadAsync<PartyMasterDto.PartyTypeDto>()).ToList();
            partyMaster.PartyContacts = (await multi.ReadAsync<PartyMasterDto.PartyContactDto>()).ToList();
            partyMaster.PartyAddresses = (await multi.ReadAsync<PartyMasterDto.PartyAddressDto>()).ToList();
            partyMaster.PartyBanks = (await multi.ReadAsync<PartyMasterDto.PartyBankDto>()).ToList();
            partyMaster.PartyDocuments = (await multi.ReadAsync<PartyMasterDto.PartyDocumentDto>()).ToList();
            return partyMaster;
        }

        public async Task<(List<GetPartyMasterDto>, int)> GetAllPartyMasterAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
                ;WITH PartyMaster_CTE AS (
                    SELECT 
                        a.Id,
                        a.PartyCode,
                        a.PartyName,
                        c.Description AS RegistrationType,
                        a.GSTNumber,
                        a.PAN,
                        a.Website,
                        STRING_AGG(CONCAT(e.PartyGroupName, '-', d.Description), ',') AS Party_GroupType,
                        a.IsActive,
                        a.PartyStatus
                    FROM Party.PartyMaster a
                    INNER JOIN Party.PartyType b ON a.Id = b.PartyId
                    INNER JOIN Party.MiscMaster c ON a.RegistrationTypeId = c.Id
                    INNER JOIN Party.MiscMaster d ON b.PartyTypeId = d.Id
                    INNER JOIN Party.PartyGroup e ON e.Id = b.PartyGroupId
                    WHERE a.IsDeleted = 0
                    GROUP BY a.Id, a.PartyCode, a.PartyName, c.Description, a.GSTNumber, a.PAN, a.Website, a.PartyStatus, a.IsActive
                )
                SELECT *,
                    COUNT(*) OVER() AS TotalCount
                FROM PartyMaster_CTE
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "WHERE (PartyName LIKE @Search OR PartyCode LIKE @Search OR Party_GroupType LIKE @Search)")}}
                ORDER BY PartyCode ASC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

            var parameters = new
            {
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var result = await _dbConnection.QueryAsync<GetPartyMasterDto, int, (GetPartyMasterDto, int)>(
                query,
                (dto, totalCount) => (dto, totalCount),
                parameters,
                splitOn: "TotalCount"
            );

            var partyMasters = result.Select(r => r.Item1).ToList();
            int totalCount = result.Any() ? result.First().Item2 : 0;

            return (partyMasters, totalCount);
        }

        public async Task<List<GetPartyMasterAutoCompleteDto>> GetPartyMasterAutoComplete(List<int> partyTypeIds,string searchPattern)
        {
                    var sql = @"
                    SELECT 
                        a.Id, 
                        a.PartyCode,
                        a.PartyName
                    FROM Party.PartyMaster a
                    INNER JOIN Party.PartyType b 
                        ON a.Id = b.PartyId
                    INNER JOIN Party.MiscMaster c 
                        ON b.PartyTypeId = c.Id
                    WHERE a.IsDeleted = 0  
                    AND a.IsActive = 1
                    /**where**/
                    GROUP BY a.Id, a.PartyCode, a.PartyName";

                // Dynamic filtering
                var filters = new List<string>
                {
                    "(a.PartyName LIKE @SearchPattern OR a.PartyCode LIKE @SearchPattern)"
                };

                if (partyTypeIds != null && partyTypeIds.Any())
                {
                    filters.Add("b.PartyTypeId IN @PartyTypeIds");
                }

                var whereClause = "AND " + string.Join(" AND ", filters);
                sql = sql.Replace("/**where**/", whereClause);

                var parameters = new
                {
                    PartyTypeIds = partyTypeIds,
                    SearchPattern = $"%{searchPattern}%"
                };

                var result = await _dbConnection.QueryAsync<GetPartyMasterAutoCompleteDto>(sql, parameters);

                return result.ToList();
        }

        
    }
}