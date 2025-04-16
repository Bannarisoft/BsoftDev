using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;
using Dapper;
using MaintenanceManagement.Infrastructure.Migrations;

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

        public async Task<(IEnumerable<dynamic> PreventiveSchedulerList, int)> GetAllPreventiveSchedulerAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
           var query = $@"
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM [Maintenance].[PreventiveSchedulerHdr] PS
                INNER JOIN [Maintenance].[MaintenanceCategory] MC ON MC.Id = PS.MaintenanceCategoryId
                INNER JOIN [Maintenance].[MiscMaster] Schedule ON Schedule.Id = PS.ScheduleId
                INNER JOIN [Maintenance].[MiscMaster] DueType ON DueType.Id = PS.DueTypeId
                INNER JOIN [Maintenance].[MiscMaster] Frequency ON Frequency.Id = PS.FrequencyId
                WHERE PS.IsDeleted = 0
                {(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (MC.CategoryName LIKE @Search OR Schedule.Code LIKE @Search OR DueType.Code LIKE @Search OR Frequency.Code LIKE @Search)")};

                SELECT  
                    PS.Id,
                    PS.DuePeriod,
                    PS.EffectiveDate,
                    PS.GraceDays,
                    PS.ReminderWorkOrderDays,
                    PS.ReminderMaterialReqDays,
                    PS.IsDownTimeRequired,
                    PS.DownTimeEstimateHrs,
                    PS.IsActive,
                    PS.CreatedBy,
                    PS.CreatedDate,
                    PS.CreatedByName,
                    PS.[CreatedIP],
                    PS.[ModifiedBy],
                    PS.[ModifiedDate],
                    PS.[ModifiedByName],
                    PS.[ModifiedIP],
                    MG.Id AS MachineGroupId,
                    MG.GroupName AS MachineGroup,
                    MC.Id AS CategoryId,
                    MC.CategoryName ,
                    Schedule.Id AS ScheduleId,
                    Schedule.Code AS Schedule,
                    DueType.Id AS DueTypeId,
                    DueType.Code AS DueType,
                    Frequency.Id AS FrequencyId,
                    Frequency.Code AS Frequency
                FROM [Maintenance].[PreventiveSchedulerHdr] PS
                INNER JOIN [Maintenance].[MachineGroup] MG ON PS.MachineGroupId = MG.Id
                INNER JOIN [Maintenance].[MaintenanceCategory] MC ON MC.Id = PS.MaintenanceCategoryId
                INNER JOIN [Maintenance].[MiscMaster] Schedule ON Schedule.Id = PS.ScheduleId
                INNER JOIN [Maintenance].[MiscMaster] DueType ON DueType.Id = PS.DueTypeId
                INNER JOIN [Maintenance].[MiscMaster] Frequency ON Frequency.Id = PS.FrequencyId
                WHERE PS.IsDeleted = 0
                {(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (MG.GroupName LIKE @Search OR MC.CategoryName LIKE @Search OR Schedule.Code LIKE @Search OR DueType.Code LIKE @Search OR Frequency.Code LIKE @Search)")}
                ORDER BY PS.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            ";

            var parameters = new
            {
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize = PageSize
            };

            using var multi = await _dbConnection.QueryMultipleAsync(query, parameters);
            var preventiveSchedulers = await multi.ReadAsync<dynamic>();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (preventiveSchedulers, totalCount);
        }

        public async Task<PreventiveSchedulerHeader> GetByIdAsync(int id)
        {
            const string query = @"
            SELECT  
                    PS.Id,
                    PS.MachineGroupId,
                    PS.DepartmentId,
                    PS.MaintenanceCategoryId,
                    PS.ScheduleId,
                    PS.DueTypeId,
                    PS.DuePeriod,
                    PS.FrequencyId,
                    PS.EffectiveDate,
                    PS.GraceDays,
                    PS.ReminderWorkOrderDays,
                    PS.ReminderMaterialReqDays,
                    PS.IsDownTimeRequired,
                    PS.DownTimeEstimateHrs,
                    PS.IsActive,
                    PSA.ActivityId,
                    PSA.EstimatedTimeHrs,
                    PSA.Description,
                    PSI.OldItemId,
                    PSI.RequiredQty
                FROM [Maintenance].[PreventiveSchedulerHdr] PS
                INNER JOIN [Maintenance].[PreventiveSchedulerActivity] PSA ON PSA.PreventiveSchedulerHdrId = PS.Id
                LEFT JOIN [Maintenance].[PreventiveSchedulerItems] PSI ON PSI.PreventiveSchedulerId = PS.Id
                WHERE PS.IsDeleted = 0 AND PS.Id = @Id";
            var PreventiveSchedulerDictionary = new Dictionary<int, PreventiveSchedulerHeader>();

    var PreventiveSchedulerResponse = await _dbConnection.QueryAsync<PreventiveSchedulerHeader,PreventiveSchedulerActivity,PreventiveSchedulerItems, PreventiveSchedulerHeader>(
        query,
        (preventiveScheduler, preventiveSchedulerActivity, preventiveSchedulerItems) =>
        {
            if (!PreventiveSchedulerDictionary.TryGetValue(preventiveScheduler.Id, out var existingPreventiveScheduler))
            {
                existingPreventiveScheduler = preventiveScheduler;
                existingPreventiveScheduler.PreventiveSchedulerActivities = new List<PreventiveSchedulerActivity>();
                existingPreventiveScheduler.PreventiveSchedulerItems = new List<PreventiveSchedulerItems>();
                PreventiveSchedulerDictionary[preventiveScheduler.Id] = existingPreventiveScheduler;
            }

                existingPreventiveScheduler.PreventiveSchedulerActivities!.Add(preventiveSchedulerActivity);
          
                existingPreventiveScheduler.PreventiveSchedulerItems!.Add(preventiveSchedulerItems);
            

            return existingPreventiveScheduler;
        },
        new { id },
        splitOn: "ActivityId,OldItemId" 
        );

            return PreventiveSchedulerResponse.FirstOrDefault()!;
        }

        public Task<List<PreventiveSchedulerHeader>> GetPreventiveScheduler(string searchPattern)
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