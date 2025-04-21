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

        public async Task<(DateTime nextDate, DateTime reminderDate)> CalculateNextScheduleDate(DateTime startDate, int interval, string unit, int reminderDays)
        {
             DateTime nextDate = unit.ToLower() switch
              {
                  "days" => startDate.AddDays(interval),
                  "months" => startDate.AddMonths(interval),
                  "years" => startDate.AddYears(interval),
                  _ => throw new ArgumentException("Unsupported frequency unit.")
              };
               DateTime reminderDate = nextDate.AddDays(-reminderDays);

            return (nextDate, reminderDate);
        }

        public async Task<(IEnumerable<dynamic> PreventiveSchedulerList, int)> GetAllPreventiveSchedulerAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
           var query = $@"
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM [Maintenance].[PreventiveSchedulerHeader] PS
                INNER JOIN [Maintenance].[MaintenanceCategory] MC ON MC.Id = PS.MaintenanceCategoryId
                INNER JOIN [Maintenance].[MiscMaster] Schedule ON Schedule.Id = PS.ScheduleId
                INNER JOIN [Maintenance].[MiscMaster] FrequencyType ON FrequencyType.Id = PS.FrequencyTypeId
                INNER JOIN [Maintenance].[MiscMaster] FrequencyUnit ON FrequencyUnit.Id = PS.FrequencyUnitId
                WHERE PS.IsDeleted = 0
                {(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (MC.CategoryName LIKE @Search OR Schedule.Code LIKE @Search OR FrequencyType.Code LIKE @Search OR FrequencyUnit.Code LIKE @Search)")};

                SELECT  
                    PS.Id,
                    PS.DepartmentId,
                    PS.FrequencyInterval,
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
                    FrequencyType.Id AS FrequencyTypeId,
                    FrequencyType.Code AS FrequencyType,
                    FrequencyUnit.Id AS FrequencyUnitId,
                    FrequencyUnit.Code AS FrequencyUnit
                FROM [Maintenance].[PreventiveSchedulerHeader] PS
                INNER JOIN [Maintenance].[MachineGroup] MG ON PS.MachineGroupId = MG.Id
                INNER JOIN [Maintenance].[MaintenanceCategory] MC ON MC.Id = PS.MaintenanceCategoryId
                INNER JOIN [Maintenance].[MiscMaster] Schedule ON Schedule.Id = PS.ScheduleId
                INNER JOIN [Maintenance].[MiscMaster] FrequencyType ON FrequencyType.Id = PS.FrequencyTypeId
                INNER JOIN [Maintenance].[MiscMaster] FrequencyUnit ON FrequencyUnit.Id = PS.FrequencyUnitId
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
                    PS.FrequencyTypeId,
                    PS.FrequencyInterval,
                    PS.FrequencyUnitId,
                    PS.EffectiveDate,
                    PS.GraceDays,
                    PS.ReminderWorkOrderDays,
                    PS.ReminderMaterialReqDays,
                    PS.IsDownTimeRequired,
                    PS.DownTimeEstimateHrs,
                    PS.IsActive,
                    PSA.Id,
                    PSA.PreventiveSchedulerHdrId,
                    PSA.ActivityId,
                    PSA.EstimatedTimeHrs,
                    PSA.Description,
                    PSI.Id,
                    PSI.PreventiveSchedulerId AS PreventiveSchedulerHdrId,
                    PSI.OldItemId,
                    PSI.RequiredQty
                FROM [Maintenance].[PreventiveSchedulerHeader] PS
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
        splitOn: "Id,Id" 
        );

            return PreventiveSchedulerResponse.FirstOrDefault()!;
        }

        public Task<List<PreventiveSchedulerHeader>> GetPreventiveScheduler(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PreventiveSchedulerDetail>> GetPreventiveSchedulerDetail(int PreventiveSchedulerId)
        {
            var query = $@"
                SELECT  
                    Id,
                    PreventiveSchedulerId,
                    MachineId,
                    WorkOrderCreationStartDate,
                    ActualWorkOrderDate,
                    RescheduleReason,
                    MaterialReqStartDays
                    
                FROM [Maintenance].[PreventiveSchedulerDetail] WHERE PreventiveSchedulerId =@PreventiveSchedulerId AND IsDeleted = 0
            ";

            var parameters = new
            {
                PreventiveSchedulerId = PreventiveSchedulerId
            };
            var result = await _dbConnection.QueryAsync<PreventiveSchedulerDetail>(query, parameters);
            
            return result.ToList();
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