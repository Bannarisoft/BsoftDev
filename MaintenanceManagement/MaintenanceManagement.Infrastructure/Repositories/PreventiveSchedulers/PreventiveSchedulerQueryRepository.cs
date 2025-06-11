using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;
using Dapper;
using MaintenanceManagement.Infrastructure.Migrations;
using static Core.Domain.Common.MiscEnumEntity;

namespace MaintenanceManagement.Infrastructure.Repositories.PreventiveSchedulers
{
    public class PreventiveSchedulerQueryRepository : IPreventiveSchedulerQuery
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
        public PreventiveSchedulerQueryRepository(IDbConnection dbConnection, IIPAddressService iPAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = iPAddressService;
        }
        public async Task<bool> AlreadyExistsAsync(int activityId, int machinegroupId, int? id = null)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var query = @"
                    SELECT COUNT(1) FROM [Maintenance].[PreventiveSchedulerHeader] PSH
                 INNER JOIN [Maintenance].[PreventiveSchedulerActivity] PSA ON PSA.PreventiveSchedulerHeaderId = PSH.Id
                   WHERE PSA.ActivityId = @ActivityId AND PSH.MachineGroupId =@MachineGroupId   AND PSH.IsDeleted = 0 AND PSH.UnitId=@UnitId ";
            var parameters = new DynamicParameters(new { ActivityId = activityId, MachineGroupId = machinegroupId, UnitId });

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
            var UnitId = _ipAddressService.GetUnitId();
            var query = $@"
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM [Maintenance].[PreventiveSchedulerHeader] PS
                INNER JOIN [Maintenance].[MachineGroup] MG ON PS.MachineGroupId = MG.Id
                INNER JOIN [Maintenance].[MiscMaster] MC ON MC.Id = PS.MaintenanceCategoryId
                INNER JOIN [Maintenance].[MiscMaster] Schedule ON Schedule.Id = PS.ScheduleId
                INNER JOIN [Maintenance].[MiscMaster] FrequencyType ON FrequencyType.Id = PS.FrequencyTypeId
                INNER JOIN [Maintenance].[MiscMaster] FrequencyUnit ON FrequencyUnit.Id = PS.FrequencyUnitId
                WHERE PS.IsDeleted = 0 AND PS.UnitId=@UnitId
                {(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (MG.GroupName LIKE @Search OR MC.Code LIKE @Search OR Schedule.Code LIKE @Search OR FrequencyType.Code LIKE @Search OR FrequencyUnit.Code LIKE @Search)")};

                SELECT  
                    PS.Id,
                    PS.DepartmentId,
                    PS.FrequencyInterval,
                    PS.PreventiveSchedulerName,
                    PS.EffectiveDate,
                    PS.GraceDays,
                    PS.ReminderWorkOrderDays,
                    PS.ReminderMaterialReqDays,
                    PS.IsDownTimeRequired,
                    PS.DownTimeEstimateHrs,
                    PS.IsActive,
                    PS.CreatedBy,
                   Cast(PS.CreatedDate as varchar)  AS CreatedDate,
                    PS.CreatedByName,
                    PS.[CreatedIP],
                    PS.[ModifiedBy],
                    PS.[ModifiedDate],
                    PS.[ModifiedByName],
                    PS.[ModifiedIP],
                    MG.Id AS MachineGroupId,
                    MG.GroupName AS MachineGroup,
                    MC.Id AS CategoryId,
                    MC.Code AS CategoryName ,
                    Schedule.Id AS ScheduleId,
                    Schedule.Code AS Schedule,
                    FrequencyType.Id AS FrequencyTypeId,
                    FrequencyType.Code AS FrequencyType,
                    FrequencyUnit.Id AS FrequencyUnitId,
                    FrequencyUnit.Code AS FrequencyUnit
                FROM [Maintenance].[PreventiveSchedulerHeader] PS
                INNER JOIN [Maintenance].[MachineGroup] MG ON PS.MachineGroupId = MG.Id
                INNER JOIN [Maintenance].[MiscMaster] MC ON MC.Id = PS.MaintenanceCategoryId
                INNER JOIN [Maintenance].[MiscMaster] Schedule ON Schedule.Id = PS.ScheduleId
                INNER JOIN [Maintenance].[MiscMaster] FrequencyType ON FrequencyType.Id = PS.FrequencyTypeId
                INNER JOIN [Maintenance].[MiscMaster] FrequencyUnit ON FrequencyUnit.Id = PS.FrequencyUnitId
                WHERE PS.IsDeleted = 0 AND PS.UnitId=@UnitId
                {(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (MG.GroupName LIKE @Search OR MC.Code LIKE @Search OR Schedule.Code LIKE @Search OR FrequencyType.Code LIKE @Search OR FrequencyUnit.Code LIKE @Search)")}
                ORDER BY PS.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            ";

            var parameters = new
            {
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize = PageSize,
                UnitId
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
                    PS.PreventiveSchedulerName,
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

            var PreventiveSchedulerResponse = await _dbConnection.QueryAsync<PreventiveSchedulerHeader, PreventiveSchedulerActivity, PreventiveSchedulerItems, PreventiveSchedulerHeader>(
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

                    if (!existingPreventiveScheduler.PreventiveSchedulerActivities!
                        .Any(a => a.Id == preventiveSchedulerActivity.Id))
                    {
                        existingPreventiveScheduler.PreventiveSchedulerActivities.Add(preventiveSchedulerActivity);
                    }

                    if (preventiveSchedulerItems != null &&
               preventiveSchedulerItems.OldItemId != null &&
               !existingPreventiveScheduler.PreventiveSchedulerItems!
                   .Any(i => i.Id == preventiveSchedulerItems.Id))
                    {
                        existingPreventiveScheduler.PreventiveSchedulerItems.Add(preventiveSchedulerItems);
                    }


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
        public async Task<DateTimeOffset?> GetLastMaintenanceDateAsync(int machineId, int PreventiveDetailId, string miscType, string misccode)
        {
            const string query = @"
                 SELECT DowntimeEnd AS EndTime   FROM [Maintenance].[WorkOrder] 
            where  PreventiveScheduleId=@PreventiveSchedulerId
            ";

            var parameters = new
            {
                PreventiveSchedulerId = PreventiveDetailId
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
        public async Task<IEnumerable<dynamic>> GetAbstractSchedulerByDate(int DepartmentId)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var statusCodes = new[] { StatusOpen.Code, GetStatusId.Status };
            var query = $@"
                            SELECT  
                                COUNT(PSD.MachineId) AS TotalScheduleCount,Cast(PSD.ActualWorkOrderDate as varchar) AS ScheduleDate,PS.DepartmentId
                            FROM [Maintenance].[PreventiveSchedulerHeader] PS
                            INNER JOIN [Maintenance].[PreventiveSchedulerDetail] PSD ON PSD.PreventiveSchedulerHeaderId = PS.Id
							LEFT JOIN Maintenance.WorkOrder WO ON WO.PreventiveScheduleId=PSD.Id
							LEFT JOIN Maintenance.MiscMaster MISC ON MISC.Id=WO.StatusId
                            WHERE PS.IsDeleted = 0 AND PSD.IsDeleted =0 AND PS.UnitId=@UnitId AND (MISC.Code IN @StatusCodes OR WO.Id IS NULL)
                            AND PSD.IsActive=1 AND PS.DepartmentId=@DepartmentId
                            GROUP BY PSD.ActualWorkOrderDate,PS.DepartmentId
                            ORDER BY PSD.ActualWorkOrderDate ASC
                        ";
            using var multi = await _dbConnection.QueryMultipleAsync(query, new { UnitId, StatusCodes = statusCodes, DepartmentId });
            var preventiveSchedulers = await multi.ReadAsync<dynamic>();

            return preventiveSchedulers;
        }
        public async Task<IEnumerable<dynamic>> GetDetailSchedulerByDate(DateOnly schedulerDate, int DepartmentId)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var statusCodes = new[] { StatusOpen.Code, GetStatusId.Status, WorkOrderHold.Code };
            var query = $@"
                            SELECT  
                                PS.Id AS HeaderId,PSD.Id AS DetailId,WO.Id AS WorkOrderId,PS.PreventiveSchedulerName,PS.MachineGroupId,MG.GroupName,PSD.MachineId,M.MachineCode,M.MachineName,PS.DepartmentId
                            FROM [Maintenance].[PreventiveSchedulerHeader] PS
                            INNER JOIN [Maintenance].[PreventiveSchedulerDetail] PSD ON PSD.PreventiveSchedulerHeaderId = PS.Id
                            INNER JOIN [Maintenance].[MachineMaster] M ON M.Id =PSD.MachineId
                            INNER JOIN [Maintenance].[MachineGroup] MG ON MG.Id = PS.MachineGroupId
                            LEFT JOIN Maintenance.WorkOrder WO ON WO.PreventiveScheduleId=PSD.Id
							LEFT JOIN Maintenance.MiscMaster MISC ON MISC.Id=WO.StatusId
                            WHERE PS.IsDeleted = 0 AND PSD.IsDeleted =0 AND PSD.ActualWorkOrderDate=@ActualWorkOrderDate AND PS.UnitId=@UnitId 
                            AND (MISC.Code IN @StatusCodes OR WO.Id IS NULL) AND PSD.IsActive=1 AND PS.DepartmentId=@DepartmentId
                            ORDER BY PS.Id ASC
                        ";
            var parameters = new
            {
                ActualWorkOrderDate = schedulerDate,
                UnitId,
                StatusCodes = statusCodes,
                DepartmentId
            };
            using var multi = await _dbConnection.QueryMultipleAsync(query, parameters);
            var preventiveSchedulers = await multi.ReadAsync<dynamic>();

            return preventiveSchedulers;
        }
        public async Task<PreventiveSchedulerHeader> GetWorkOrderScheduleDetailById(int Id)
        {
            var query = $@"
                    SELECT  
						 PSH.Id,
						PSH.CompanyId,
                       PSH.UnitId,
                       PSH.MaintenanceCategoryId,
                       PSD.Id,
                       PSA.ActivityId,
                       PSI.OldItemId
					   FROM [Maintenance].[PreventiveSchedulerDetail] PSD
				   INNER JOIN [Maintenance].[PreventiveSchedulerHeader] PSH ON PSD.PreventiveSchedulerHeaderId=PSH.Id
				   INNER JOIN [Maintenance].[PreventiveSchedulerActivity] PSA ON PSA.PreventiveSchedulerHeaderId=PSD.PreventiveSchedulerHeaderId
				   LEFT JOIN [Maintenance].[PreventiveSchedulerItems] PSI ON PSI.PreventiveSchedulerHeaderId=PSD.PreventiveSchedulerHeaderId
				   WHERE PSD.Id =@Id AND PSD.IsDeleted = 0  AND PSH.IsDeleted = 0
               ";

            var PreventiveSchedulerDictionary = new Dictionary<int, PreventiveSchedulerHeader>();

            var PreventiveSchedulerResponse = await _dbConnection.QueryAsync<PreventiveSchedulerHeader, PreventiveSchedulerDetail, PreventiveSchedulerActivity, PreventiveSchedulerItems, PreventiveSchedulerHeader>(
            query,
            (preventiveScheduler, preventiveSchedulerDetail, preventiveSchedulerActivity, preventiveSchedulerItems) =>
            {
                if (!PreventiveSchedulerDictionary.TryGetValue(preventiveScheduler.Id, out var existingPreventiveScheduler))
                {
                    existingPreventiveScheduler = preventiveScheduler;
                    existingPreventiveScheduler.PreventiveSchedulerDetails = new List<PreventiveSchedulerDetail>();
                    existingPreventiveScheduler.PreventiveSchedulerActivities = new List<PreventiveSchedulerActivity>();
                    existingPreventiveScheduler.PreventiveSchedulerItems = new List<PreventiveSchedulerItems>();
                    PreventiveSchedulerDictionary[preventiveScheduler.Id] = existingPreventiveScheduler;
                }

                existingPreventiveScheduler.PreventiveSchedulerDetails!.Add(preventiveSchedulerDetail);

                existingPreventiveScheduler.PreventiveSchedulerActivities!.Add(preventiveSchedulerActivity);
                if (preventiveSchedulerItems != null && preventiveSchedulerItems.OldItemId != null)
                {
                    existingPreventiveScheduler.PreventiveSchedulerItems!.Add(preventiveSchedulerItems);
                }


                return existingPreventiveScheduler;
            },
            new { Id },
            splitOn: "Id,ActivityId,OldItemId"
            );

            return PreventiveSchedulerResponse.FirstOrDefault()!;
        }
        public async Task<bool> MachingroupValidation(int id)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var query = @"SELECT COUNT(1) FROM [Maintenance].[MachineGroup] MG
                 INNER JOIN [Maintenance].[MachineMaster] MM ON MM.MachineGroupId=MG.Id
                  WHERE MG.Id = @Id AND MG.IsDeleted = 0 AND MM.Unitid=@UnitId";

            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id, UnitId });
            return count > 0;
        }
        public async Task<bool> ExistWorkOrderBySchedulerDetailId(int id)
        {

            var query = @"SELECT COUNT(1) FROM [Maintenance].[WorkOrder]
                  WHERE PreventiveScheduleId = @Id ";

            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
            return count > 0;
        }

        public async Task<bool> ExistPreventivescheduleItem(int id)
        {
            var query = @"SELECT COUNT(*) FROM [Maintenance].[PreventiveSchedulerDetail] PSD
				   INNER JOIN [Maintenance].[PreventiveSchedulerItems] PSI ON PSI.PreventiveSchedulerHeaderId=PSD.PreventiveSchedulerHeaderId
				   WHERE PSD.Id =@Id  AND PSD.IsDeleted = 0 ";

            var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
            return count > 0;
        }

        public async Task<PreventiveSchedulerHeader> GetWorkOrderScheduleDetailWithoutItemidById(int Id)
        {
            var query = $@"
                    SELECT  
						 PSH.Id,
						PSH.CompanyId,
                       PSH.UnitId,
                       PSH.MaintenanceCategoryId,
                       PSD.Id,
                       PSA.ActivityId
					   FROM [Maintenance].[PreventiveSchedulerDetail] PSD
				   INNER JOIN [Maintenance].[PreventiveSchedulerHeader] PSH ON PSD.PreventiveSchedulerHeaderId=PSH.Id
				   INNER JOIN [Maintenance].[PreventiveSchedulerActivity] PSA ON PSA.PreventiveSchedulerHeaderId=PSD.PreventiveSchedulerHeaderId
				   WHERE PSD.Id =@Id AND PSD.IsDeleted = 0  AND PSH.IsDeleted = 0
               ";

            var PreventiveSchedulerDictionary = new Dictionary<int, PreventiveSchedulerHeader>();

            var PreventiveSchedulerResponse = await _dbConnection.QueryAsync<PreventiveSchedulerHeader, PreventiveSchedulerDetail, PreventiveSchedulerActivity, PreventiveSchedulerHeader>(
            query,
            (preventiveScheduler, preventiveSchedulerDetail, preventiveSchedulerActivity) =>
            {
                if (!PreventiveSchedulerDictionary.TryGetValue(preventiveScheduler.Id, out var existingPreventiveScheduler))
                {
                    existingPreventiveScheduler = preventiveScheduler;
                    existingPreventiveScheduler.PreventiveSchedulerDetails = new List<PreventiveSchedulerDetail>();
                    existingPreventiveScheduler.PreventiveSchedulerActivities = new List<PreventiveSchedulerActivity>();
                    PreventiveSchedulerDictionary[preventiveScheduler.Id] = existingPreventiveScheduler;
                }

                existingPreventiveScheduler.PreventiveSchedulerDetails!.Add(preventiveSchedulerDetail);

                existingPreventiveScheduler.PreventiveSchedulerActivities!.Add(preventiveSchedulerActivity);


                return existingPreventiveScheduler;
            },
            new { Id },
            splitOn: "Id,ActivityId"
            );

            return PreventiveSchedulerResponse.FirstOrDefault()!;
        }
        public async Task<Core.Domain.Entities.MachineGroup> GetMachineGroupIdByName(string MachineGroupName)
        {
            const string query = @"
                       SELECT Id, GroupName  
                       FROM Maintenance.MachineGroup
                       WHERE IsDeleted = 0 AND GroupName = @SearchPattern";

            var parameters = new
            {
                SearchPattern = MachineGroupName
            };
            var machineGroups = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineGroup>(query, parameters);
            return machineGroups.FirstOrDefault();
        }

        public async Task<Core.Domain.Entities.MachineMaster> GetMachineIdByCode(string MachineCode)
        {
            const string query = @"
                       SELECT Id, MachineCode  
                       FROM [Maintenance].[MachineMaster]
                       WHERE IsDeleted = 0 AND MachineCode = @SearchPattern";

            var parameters = new
            {
                SearchPattern = MachineCode
            };
            var machines = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineMaster>(query, parameters);
            return machines.FirstOrDefault();
        }
         public async Task<PreventiveSchedulerDetail> GetPreventiveSchedulerDetailByName(string PreventiveSchedulerName,string MachineId)
        {
            var query = $@"
                SELECT  
                    PSD.Id,
                    PSD.PreventiveSchedulerHeaderId,
                    PSD.MachineId,
                    PSD.WorkOrderCreationStartDate,
                    PSD.ActualWorkOrderDate,
                    PSD.RescheduleReason,
                    PSD.MaterialReqStartDays,
                    PSD.HangfireJobId
                    
                FROM [Maintenance].[PreventiveSchedulerDetail] PSD
        INNER JOIN [Maintenance].[PreventiveSchedulerHeader] PSH ON PSD.PreventiveSchedulerHeaderId=PSH.Id
        WHERE PreventiveSchedulerName =@PreventiveSchedulerName AND PSD.MachineId=@MachineId AND PSD.IsDeleted = 0
            ";

            var parameters = new
            {
                PreventiveSchedulerName,
                 MachineId
            };
            var result = await _dbConnection.QueryAsync<PreventiveSchedulerDetail>(query, parameters);

            return result.FirstOrDefault();
        }

        public async Task<Core.Domain.Entities.ActivityMaster> GetActivityIdByName(string ActivityName)
        {
            const string query = @"
                       SELECT Id, ActivityName  
                       FROM Maintenance.ActivityMaster
                       WHERE IsDeleted = 0 AND IsActive = 1 AND ActivityName = @SearchPattern";

            var parameters = new
            {
                SearchPattern = ActivityName
            };
            var machineGroups = await _dbConnection.QueryAsync<Core.Domain.Entities.ActivityMaster>(query, parameters);
            return machineGroups.FirstOrDefault();
        }

        public async Task<IEnumerable<dynamic>> GetDetailSchedulerByPreventiveScheduleId(int Id)
        {
            var UnitId = _ipAddressService.GetUnitId();
            var statusCodes = new[] { StatusOpen.Code, GetStatusId.Status, WorkOrderHold.Code };
            var query = $@"
                            SELECT  
                                PS.Id AS HeaderId,PSD.Id AS DetailId,WO.Id AS WorkOrderId,PS.PreventiveSchedulerName,PS.MachineGroupId,MG.GroupName,PSD.MachineId,M.MachineCode,M.MachineName,PS.DepartmentId
                            FROM [Maintenance].[PreventiveSchedulerHeader] PS
                            INNER JOIN [Maintenance].[PreventiveSchedulerDetail] PSD ON PSD.PreventiveSchedulerHeaderId = PS.Id
                            INNER JOIN [Maintenance].[MachineMaster] M ON M.Id =PSD.MachineId
                            INNER JOIN [Maintenance].[MachineGroup] MG ON MG.Id = PS.MachineGroupId
                            LEFT JOIN Maintenance.WorkOrder WO ON WO.PreventiveScheduleId=PSD.Id
							LEFT JOIN Maintenance.MiscMaster MISC ON MISC.Id=WO.StatusId
                            WHERE PS.IsDeleted = 0 AND PSD.IsDeleted =0 AND PS.Id=@PreventiveScheduleId AND PS.UnitId=@UnitId 
                            AND (MISC.Code IN @StatusCodes OR WO.Id IS NULL) AND PSD.IsActive=1 
                            ORDER BY PS.Id ASC
                        ";
            var parameters = new
            {
                PreventiveScheduleId = Id,
                UnitId,
                StatusCodes = statusCodes
            };
            using var multi = await _dbConnection.QueryMultipleAsync(query, parameters);
            var preventiveSchedulers = await multi.ReadAsync<dynamic>();

            return preventiveSchedulers;
        }
    }
}