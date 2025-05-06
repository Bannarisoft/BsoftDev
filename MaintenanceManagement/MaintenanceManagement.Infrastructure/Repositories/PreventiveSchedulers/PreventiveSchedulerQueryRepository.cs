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
        public async Task<bool> AlreadyExistsAsync(int activityId,int machinegroupId,int? id = null)
        {
            var query = @"
                    SELECT COUNT(1) FROM [Maintenance].[PreventiveSchedulerHeader] PSH
                 INNER JOIN [Maintenance].[PreventiveSchedulerActivity] PSA ON PSA.PreventiveSchedulerHeaderId = PSH.Id
                   WHERE PSA.ActivityId = @ActivityId AND PSH.MachineGroupId =@MachineGroupId   AND PSH.IsDeleted = 0";
                var parameters = new DynamicParameters(new { ActivityId = activityId, MachineGroupId =machinegroupId });

             if (id is not null)
             {
                 query += " AND PSH.Id != @Id";
                 parameters.Add("Id", id);
             }
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
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
                    PSA.PreventiveSchedulerHeaderId,
                    PSA.ActivityId,
                    PSI.Id,
                    PSI.PreventiveSchedulerHeaderId,
                    PSI.OldItemId,
                    PSI.RequiredQty,
                    PSI.OldCategoryDescription,
                    PSI.OldGroupName
                FROM [Maintenance].[PreventiveSchedulerHeader] PS
                INNER JOIN [Maintenance].[PreventiveSchedulerActivity] PSA ON PSA.PreventiveSchedulerHeaderId = PS.Id
                LEFT JOIN [Maintenance].[PreventiveSchedulerItems] PSI ON PSI.PreventiveSchedulerHeaderId = PS.Id
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
                    PreventiveSchedulerHeaderId,
                    MachineId,
                    WorkOrderCreationStartDate,
                    ActualWorkOrderDate,
                    RescheduleReason,
                    MaterialReqStartDays,
                    HangfireJobId
                    
                FROM [Maintenance].[PreventiveSchedulerDetail] WHERE PreventiveSchedulerHeaderId =@PreventiveSchedulerId AND IsDeleted = 0
            ";

            var parameters = new
            {
                PreventiveSchedulerId = PreventiveSchedulerId
            };
            var result = await _dbConnection.QueryAsync<PreventiveSchedulerDetail>(query, parameters);
            
            return result.ToList();
        }

        public async Task<bool> NotFoundAsync(int id)
        {
            var query = "SELECT COUNT(1) FROM [Maintenance].[PreventiveSchedulerHeader] WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
        }

        public Task<bool> SoftDeleteValidation(int Id)
        {
            throw new NotImplementedException();
        }
       public async Task<DateTimeOffset?> GetLastMaintenanceDateAsync(int machineId,int PreventiveSchedulerId,string miscType,string misccode)
        {
            const string query = @"
                 SELECT MAX(WS.EndTime) AS EndTime   FROM [Maintenance].[WorkOrder] W
            INNER JOIN [Maintenance].[WorkOrderSchedule] WS ON W.Id=WS.WorkOrderId
            INNER JOIN Maintenance.MiscMaster M ON W.StatusId=M.Id
            INNER JOIN Maintenance.MiscTypeMaster MT ON MT.Id =M.MiscTypeId
            INNER JOIN [Maintenance].[PreventiveSchedulerDetail] PSD ON PSD.Id=W.PreventiveScheduleId
            where MT.MiscTypeCode=@StatusTypeCode AND M.Code=@StatusCode AND PSD.MachineId=@MachineId AND PSD.PreventiveSchedulerHeaderId=@PreventiveSchedulerId
            ";

            var parameters = new
            {
                MachineId = machineId,
                StatusTypeCode = miscType,
                StatusCode = misccode,
                PreventiveSchedulerId =PreventiveSchedulerId
            };

            var result = await _dbConnection.QueryFirstOrDefaultAsync<DateTimeOffset?>(query, parameters);
            return result;
        }

            public async Task<PreventiveSchedulerDetail> GetPreventiveSchedulerDetailById(int Id)
           {
               var query = $@"
                   SELECT  
                       Id,
                       PreventiveSchedulerId,
                       MachineId,
                       WorkOrderCreationStartDate,
                       ActualWorkOrderDate,
                       RescheduleReason,
                       MaterialReqStartDays,
                       HangfireJobId
                       
                   FROM [Maintenance].[PreventiveSchedulerDetail] WHERE Id =@Id AND IsDeleted = 0
               ";
    
               var parameters = new
               {
                   Id = Id
               };
               var result = await _dbConnection.QueryFirstOrDefaultAsync<PreventiveSchedulerDetail>(query, parameters);
               
               return result;
           }
             public async Task<bool> UpdateValidation(int id)
             {
                 var query = @"SELECT COUNT(1) FROM [Maintenance].[PreventiveSchedulerHeader] PSH
                 INNER JOIN [Maintenance].[PreventiveSchedulerDetail] PSD ON PSD.PreventiveSchedulerHeaderId=PSH.Id
                INNER JOIN [Maintenance].[WorkOrder] W ON W.PreventiveScheduleId =PSD.Id
                  WHERE PSH.Id = @Id AND PSH.IsDeleted = 0";

                     var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
                     return count > 0;
             }
               public async Task<IEnumerable<dynamic>> GetAbstractSchedulerByDate()
                {
                       var query = $@"
                            SELECT  
                                COUNT(PSD.MachineId) AS TotalScheduleCount,Cast(PSD.ActualWorkOrderDate as varchar) AS ScheduleDate
                            FROM [Maintenance].[PreventiveSchedulerHeader] PS
                            INNER JOIN [Maintenance].[PreventiveSchedulerDetail] PSD ON PSD.PreventiveSchedulerHeaderId = PS.Id
                            WHERE PS.IsDeleted = 0 AND PSD.IsDeleted =0 
                            GROUP BY PSD.ActualWorkOrderDate
                            ORDER BY PSD.ActualWorkOrderDate ASC
                        ";
                         using var multi = await _dbConnection.QueryMultipleAsync(query);
                        var preventiveSchedulers = await multi.ReadAsync<dynamic>();

                        return preventiveSchedulers;
                }
                 public async Task<IEnumerable<dynamic>> GetDetailSchedulerByDate(DateOnly schedulerDate)
                {
                       var query = $@"
                            SELECT  
                                PS.Id,PS.MachineGroupId,MG.GroupName,PSD.MachineId,M.MachineName
                            FROM [Maintenance].[PreventiveSchedulerHeader] PS
                            INNER JOIN [Maintenance].[PreventiveSchedulerDetail] PSD ON PSD.PreventiveSchedulerHeaderId = PS.Id
                            INNER JOIN [Maintenance].[MachineMaster] M ON M.Id =PSD.MachineId
                            INNER JOIN [Maintenance].[MachineGroup] MG ON MG.Id = PS.MachineGroupId
                            WHERE PS.IsDeleted = 0 AND PSD.IsDeleted =0 AND PSD.ActualWorkOrderDate=@ActualWorkOrderDate
                            ORDER BY PS.Id ASC
                        ";
                        var parameters = new
                        {
                            ActualWorkOrderDate = schedulerDate
                        };
                         using var multi = await _dbConnection.QueryMultipleAsync(query,parameters);
                        var preventiveSchedulers = await multi.ReadAsync<dynamic>();

                        return preventiveSchedulers;
                }
             

    }
}