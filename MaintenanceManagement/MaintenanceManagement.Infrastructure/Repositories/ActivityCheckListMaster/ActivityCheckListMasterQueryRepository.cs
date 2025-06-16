using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Queries.GetCheckListByActivityId;
using Core.Application.Common.Interfaces.IActivityCheckListMaster;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.ActivityCheckListMaster
{
    public class ActivityCheckListMasterQueryRepository : IActivityCheckListMasterQueryRepository
    {
        private readonly IDbConnection _dbConnection;


        public ActivityCheckListMasterQueryRepository(IDbConnection dbConnection)
        {

            _dbConnection = dbConnection;

        }



        public async Task<(List<GetAllActivityCheckListMasterDto>, int)> GetAllActivityCheckListMasterAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
                    DECLARE @TotalCount INT;
                    SELECT @TotalCount = COUNT(DISTINCT aclm.Id)
                    FROM Maintenance.Maintenance.ActivityCheckListMaster aclm
                    INNER JOIN Maintenance.ActivityMaster am 
                        ON aclm.ActivityID = am.Id
                    WHERE aclm.IsDeleted = 0
                    {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (am.ActivityName LIKE @Search OR aclm.ActivityChecklist LIKE @Search)")}};

                    SELECT 
                        aclm.Id AS ChecklistId,
                        aclm.ActivityID,
                        am.ActivityName,
                        aclm.ActivityChecklist ,
                        aclm.IsActive,
                        aclm.IsDeleted,
                        aclm.CreatedBy,
                        aclm.CreatedDate,
                        aclm.CreatedByName,
                        aclm.CreatedIP,
                        aclm.ModifiedBy,
                        aclm.ModifiedDate,
                        aclm.ModifiedByName,
                        aclm.ModifiedIP
                    FROM Maintenance.ActivityCheckListMaster aclm
                    INNER JOIN Maintenance.ActivityMaster am 
                        ON aclm.ActivityID = am.Id
                    WHERE aclm.IsDeleted = 0
                    {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (am.ActivityName LIKE @Search OR aclm.ActivityChecklist LIKE @Search)")}}
                    ORDER BY aclm.Id DESC 
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                    SELECT @TotalCount AS TotalCount;
                """;

            var parameters = new
            {
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var result = await _dbConnection.QueryMultipleAsync(query, parameters);
            var checkListMasters = (await result.ReadAsync<GetAllActivityCheckListMasterDto>()).ToList();
            int totalCount = await result.ReadFirstAsync<int>();

            return (checkListMasters, totalCount);
        }

        public async Task<GetAllActivityCheckListMasterDto> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT 
                    aclm.Id AS ChecklistId,
                    aclm.ActivityID,
                    am.ActivityName,
                    aclm.ActivityChecklist  ,
                    aclm.IsActive,
                    aclm.IsDeleted,
                    aclm.CreatedBy,
                    aclm.CreatedDate,
                    aclm.CreatedByName,
                    aclm.CreatedIP,
                    aclm.ModifiedBy,
                    aclm.ModifiedDate,
                    aclm.ModifiedByName,
                    aclm.ModifiedIP
                FROM Maintenance.ActivityCheckListMaster aclm
                INNER JOIN Maintenance.ActivityMaster am 
                    ON aclm.ActivityID = am.Id
                WHERE aclm.Id = @id AND aclm.IsDeleted = 0";

            return await _dbConnection.QueryFirstOrDefaultAsync<GetAllActivityCheckListMasterDto>(query, new { id });
        }


        public async Task<bool> GetByActivityCheckListAsync(string activityCheckList, int activityId)
        {
            var query = """
            SELECT COUNT(1) FROM Maintenance.ActivityCheckListMaster
            WHERE ActivityCheckList = @activityCheckList AND ActivityID = @activityId  AND IsDeleted = 0 
            """;

            var result = await _dbConnection.ExecuteScalarAsync<int>(query, new { ActivityCheckList = activityCheckList, ActivityID = activityId });

            return result > 0;
        }

        public async Task<bool> AlreadyExistsCheckListAsync(string activityCheckList, int activityId, int? id = null)
        {

            var query = "SELECT COUNT(1) FROM Maintenance.ActivityCheckListMaster WHERE ActivityCheckList = @activityCheckList AND ActivityID = @activityId AND IsDeleted = 0";
            var parameters = new DynamicParameters(new { ActivityCheckList = activityCheckList, ActivityID = activityId });

            if (id is not null)
            {
                query += " AND Id != @Id";
                parameters.Add("Id", id);
            }
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
            return count > 0;
        }
        public async Task<List<GetActivityCheckListByActivityIdDto>> GetCheckListByActivityIdsAsync(List<int> ids, int? workOrderId = null)
        {
            if (ids == null || !ids.Any())
            {
                return new List<GetActivityCheckListByActivityIdDto>();
            }

            const string query = @"
                    SELECT 
                        aclm.Id AS ChecklistId,
                        aclm.ActivityID,
                        am.ActivityName,
                        aclm.ActivityChecklist,
                        aclm.IsActive,
                        aclm.IsDeleted,
                        aclm.CreatedBy,
                        aclm.CreatedDate,
                        aclm.CreatedByName,
                        aclm.CreatedIP,
                        aclm.ModifiedBy,
                        aclm.ModifiedDate,
                        aclm.ModifiedByName,
                        aclm.ModifiedIP
                    FROM Maintenance.ActivityCheckListMaster aclm
                    INNER JOIN Maintenance.ActivityMaster am ON aclm.ActivityID = am.Id
                    LEFT JOIN Maintenance.WorkOrderCheckList WOC 
                        ON WOC.CheckListId = aclm.Id 
                        AND (@WorkOrderId IS NULL OR WOC.WorkOrderId = @WorkOrderId)
                    WHERE aclm.ActivityID IN @Ids
                        AND aclm.IsDeleted = 0
                    GROUP BY 
                        aclm.Id, aclm.ActivityID, am.ActivityName, aclm.ActivityChecklist, aclm.IsActive, 
                        aclm.IsDeleted, aclm.CreatedBy, aclm.CreatedDate, aclm.CreatedByName, aclm.CreatedIP,
                        aclm.ModifiedBy, aclm.ModifiedDate, aclm.ModifiedByName, aclm.ModifiedIP";

            var parameters = new
            {
                Ids = ids,
                WorkOrderId = workOrderId
            };

            var result = await _dbConnection.QueryAsync<GetActivityCheckListByActivityIdDto>(query, parameters);
            return result.ToList();
        }
            
            

        //    public async Task<List<GetActivityCheckListByActivityIdDto>> GetCheckListByActivityIdsAsync(List<int> ids)
        //     {
        //          if (ids == null || !ids.Any())
        //         {
        //             return new List<GetActivityCheckListByActivityIdDto>();
        //         }
        //         const string query = @"
        //             SELECT 
        //                 aclm.Id AS ChecklistId,
        //                 aclm.ActivityID,
        //                 am.ActivityName,
        //                 aclm.ActivityChecklist  ,
        //                 aclm.IsActive,
        //                 aclm.IsDeleted,
        //                 aclm.CreatedBy,
        //                 aclm.CreatedDate,
        //                 aclm.CreatedByName,
        //                 aclm.CreatedIP,
        //                 aclm.ModifiedBy,
        //                 aclm.ModifiedDate,
        //                 aclm.ModifiedByName,
        //                 aclm.ModifiedIP
        //             FROM Maintenance.ActivityCheckListMaster aclm
        //             INNER JOIN Maintenance.ActivityMaster am 
        //                 ON aclm.ActivityID = am.Id
        //             WHERE aclm.ActivityID IN  @ids AND aclm.IsDeleted = 0";

        //         var result = await _dbConnection.QueryAsync<GetActivityCheckListByActivityIdDto>(query, new { ids });
        //         return result.ToList();
        //     }



    }
}