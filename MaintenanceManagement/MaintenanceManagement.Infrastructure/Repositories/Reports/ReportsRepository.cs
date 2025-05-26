using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IReports;
using Core.Application.Reports.GetStockLegerReport;
using Core.Application.Reports.MaintenanceRequestReport;
using Core.Application.Reports.WorkOrderItemConsuption;
using Core.Application.Reports.WorkOrderReport;
using Core.Application.StockLedger.Queries.GetCurrentStock;
using Core.Application.Reports.WorkOderCheckListReport;
using Core.Application.WorkOrder.Command.CreateWorkOrder;
using Dapper;
using MaintenanceManagement.Infrastructure.Repositories.Common;
using Core.Application.Reports.MRS;

namespace MaintenanceManagement.Infrastructure.Repositories.Reports
{

    public class ReportsRepository : BaseQueryRepository, IReportRepository
    {

        private readonly IDbConnection _dbConnection;
        public ReportsRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)

            : base(ipAddressService)
        {

            _dbConnection = dbConnection;
        }


        public async Task<List<RequestReportDto>> MaintenanceReportAsync(DateTimeOffset? requestFromDate, DateTimeOffset? requestToDate, int? requestType, int? requestStatus, int? departmentId)
         {
            var parameters = new DynamicParameters();

            if (requestFromDate.HasValue)
                parameters.Add("RequestFromDate", requestFromDate.Value.Date);

            if (requestToDate.HasValue)
                parameters.Add("RequestToDate", requestToDate.Value.Date.AddDays(1).AddTicks(-1));

            if (requestType.HasValue)
                parameters.Add("RequestType", requestType.Value);

            if (requestStatus.HasValue)
                parameters.Add("RequestStatus", requestStatus.Value);

            if (departmentId.HasValue)
                parameters.Add("DepartmentId", departmentId.Value);

            var result = await _dbConnection.QueryAsync<RequestReportDto>(
                "[dbo].[Rpt_GetMaintenanceRequestReport]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<List<WorkOrderReportDto>> WorkOrderReportAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate, int? RequestTypeId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);
            parameters.Add("@UnitId", UnitId);
            parameters.Add("@FromDate", fromDate);
            parameters.Add("@Todate", toDate);
            parameters.Add("@RequestType", RequestTypeId);         
          
            var result = await _dbConnection.QueryAsync<WorkOrderReportDto>(
                "dbo.Rpt_WorkOrderReport", 
                parameters, 
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120);
                
            return result.ToList(); 
        }

        public async Task<List<WorkOrderIssueDto>> GetItemConsumptionAsync(DateTimeOffset IssueFromDate, DateTimeOffset IssueToDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", IssueFromDate);
            parameters.Add("@ToDate", IssueToDate);
            parameters.Add("@UnitId", UnitId);
           

            var result = await _dbConnection.QueryAsync<WorkOrderIssueDto>(
                "GetItemConsumptionDetails",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<List<StockLedgerReportDto>> GetSubStoresStockLedger(string OldUnitcode, DateTime FromDate, DateTime ToDate, string? Itemcode)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", FromDate);
            parameters.Add("@ToDate", ToDate);
            parameters.Add("@ItemCode", Itemcode);
            parameters.Add("@OldUnitCode", OldUnitcode);

            var result = await _dbConnection.QueryAsync<StockLedgerReportDto>(
                "GetSubStoreStockLedgerSummary",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
        
         public async Task<List<CurrentStockDto>> GetStockDetails(string OldUnitcode)
        {
             OldUnitcode = OldUnitcode ?? string.Empty; // Prevent null issues

            const string query = @"
                SELECT 
                    Oldunitcode as OldUnitId,
                    ItemCode,
                    ItemName,
					Uom,
                    SUM(ReceivedQty) - SUM(IssueQty) AS StockQty,
                    SUM(ReceivedValue) - SUM(IssueValue) AS StockValue,
                    ((SUM(ReceivedValue) - SUM(IssueValue)) / (SUM(ReceivedQty) - SUM(IssueQty))) AS Rate
                FROM 
                    Maintenance.StockLedger
                WHERE
                    Oldunitcode = @OldUnitcode 
                    AND TransactionType not in('SRP')
                GROUP BY 
                    ItemCode, ItemName, Oldunitcode,Uom
                HAVING
                    SUM(ReceivedQty) - SUM(IssueQty) > 0";

            var parameters = new 
            { 
                OldUnitcode // match exactly, no wildcards
            };

            var itemcodes = await _dbConnection.QueryAsync<CurrentStockDto>(query, parameters);
            return itemcodes.ToList();
        }
		 public async Task<List<WorkOderCheckListReportDto>> GetWorkOrderChecklistReportAsync(
                         DateTimeOffset? fromDate,
                        DateTimeOffset? toDate,
                        int? machineGroupId ,
                        int? machineId ,
                        int? activityId 
                       )
                {
                    var parameters = new DynamicParameters();

                    if (fromDate.HasValue)
                        parameters.Add("FromDate", fromDate.Value.Date);

                    if (toDate.HasValue)
                        parameters.Add("ToDate", toDate.Value.Date.AddDays(1).AddTicks(-1)); // include full day

                    if (machineGroupId.HasValue)
                        parameters.Add("MachineGroupId", machineGroupId.Value);

                    if (machineId.HasValue)
                        parameters.Add("MachineId", machineId.Value);

                    if (activityId.HasValue)
                        parameters.Add("ActivityId", activityId.Value);                    
                   
                        parameters.Add("UnitId", UnitId);       

                    var result = await _dbConnection.QueryAsync<WorkOderCheckListReportDto>(
                        "[dbo].[Rpt_GetWorkOrderChecklistDetails]",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.ToList();
                }

        public async Task<List<MRSReportDto>> GetMRSReports(DateTimeOffset IssueFromDate, DateTimeOffset IssueToDate, string OldUnitCode)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", IssueFromDate);
            parameters.Add("@ToDate", IssueToDate);
            parameters.Add("@OldUnitId", OldUnitCode);
            var result = await _dbConnection.QueryAsync<MRSReportDto>(
                "GetMRSReport",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }

        public async Task<IEnumerable<dynamic>> ScheduleReportAsync(DateTime? FromDueDate, DateTime? ToDueDate)
        {
            var query = $$"""

                Select PSH.DepartmentId,MG.GroupName,MM.MachineName,MISC.description AS MaintenanceCategory,A.ActivityName,ActivityType.Code AS ActivityType,
                Cast(PSD.ActualWorkOrderDate as varchar) AS DueDate,PSD.LastMaintenanceActivityDate from [Maintenance].[PreventiveSchedulerHeader] PSH
                Inner Join [Maintenance].[MachineGroup] MG ON PSH.MachineGroupId=MG.Id
                Inner Join [Maintenance].[PreventiveSchedulerDetail] PSD ON PSD.PreventiveSchedulerHeaderId=PSH.Id
                Inner Join [Maintenance].[PreventiveSchedulerActivity] PSA ON PSA.PreventiveSchedulerHeaderId=PSH.Id
                Inner Join [Maintenance].[MachineMaster] MM ON MM.Id=PSD.MachineId
                Inner Join [Maintenance].[MiscMaster] MISC ON MISC.Id=PSH.MaintenanceCategoryId
                Inner Join [Maintenance].[ActivityMaster] A ON A.Id=PSA.ActivityId
                Inner Join [Maintenance].[MiscMaster] ActivityType ON ActivityType.Id=A.ActivityType
                WHERE PSH.IsDeleted=0 AND PSD.IsActive=1
                {{(FromDueDate.HasValue ? "AND PSD.ActualWorkOrderDate >= @FromDueDate" : "")}}
                {{(ToDueDate.HasValue ? "AND PSD.ActualWorkOrderDate <= @ToDueDate" : "")}}
                group by PSH.Id,PSH.DepartmentId,MG.GroupName,MISC.description,A.ActivityName,ActivityType.Code,PSD.ActualWorkOrderDate,PSD.LastMaintenanceActivityDate,MM.MachineName 
                
                ORDER BY PSH.Id desc
            """;
            
              var parameters = new
                       {
                           FromDueDate = FromDueDate,
                           ToDueDate  = ToDueDate ,
                       };

             var schedule = await _dbConnection.QueryMultipleAsync(query, parameters);
             var schedulelist = await schedule.ReadAsync<dynamic>();

            return schedulelist;
        }

        public async Task<IEnumerable<dynamic>> MaterialPlanningReportAsync(DateTime? FromDueDate, DateTime? ToDueDate)
        {
                   var query = $$"""
           SELECT 
               ItemCode, 
               UOM, 
               CategoryDescription, 
               GroupName 
           INTO #stock 
           FROM [Maintenance].[SubStores] SS
           INNER JOIN [Maintenance].[PreventiveSchedulerItems] PSI 
               ON SS.ItemCode = PSI.OldItemId 
               AND PSI.OldCategoryDescription = SS.CategoryDescription 
               AND PSI.OldGroupName = SS.GroupName;

           SELECT 
               SS.ItemCode, 
               SS.UOM, 
               SUM(SS.Quantity) AS TotQty, 
               SS.CategoryDescription, 
               SS.GroupName 
           INTO #STOCKQTY 
           FROM [Maintenance].[SubStores] SS
           INNER JOIN #stock S 
               ON SS.ItemCode = S.ItemCode 
               AND SS.CategoryDescription = S.CategoryDescription 
               AND SS.UOM = S.UOM 
               AND SS.GroupName = S.GroupName 
           GROUP BY 
               SS.ItemCode, 
               SS.UOM, 
               SS.CategoryDescription, 
               SS.GroupName;

           SELECT 
               MM.MachineName,
               MISC.Description AS MaintenanceCategory,
               A.ActivityName,
               ActivityType.Code AS ActivityType,
               CAST(PSD.ActualWorkOrderDate AS VARCHAR) AS PlannedMaintenanceDate,
               PSI.OldItemId AS MaterialCode,
               PSI.OldCategoryDescription AS MaterialDescription,
               SQ.UOM,
               ISNULL(SQ.TotQty, 0) AS CurrentStock,
               PSI.RequiredQty,
               ISNULL(SQ.TotQty, 0) - PSI.RequiredQty AS Shortfall_Excess
           FROM [Maintenance].[PreventiveSchedulerHeader] PSH
           INNER JOIN [Maintenance].[MachineGroup] MG 
               ON PSH.MachineGroupId = MG.Id
           INNER JOIN [Maintenance].[PreventiveSchedulerDetail] PSD 
               ON PSD.PreventiveSchedulerHeaderId = PSH.Id
           INNER JOIN [Maintenance].[PreventiveSchedulerActivity] PSA 
               ON PSA.PreventiveSchedulerHeaderId = PSH.Id
           INNER JOIN [Maintenance].[PreventiveSchedulerItems] PSI 
               ON PSI.PreventiveSchedulerHeaderId = PSH.Id
           INNER JOIN [Maintenance].[MachineMaster] MM 
               ON MM.MachineGroupId = MG.Id
           INNER JOIN [Maintenance].[MiscMaster] MISC 
               ON MISC.Id = PSH.MaintenanceCategoryId
           INNER JOIN [Maintenance].[ActivityMaster] A 
               ON A.Id = PSA.ActivityId
           INNER JOIN [Maintenance].[MiscMaster] ActivityType 
               ON ActivityType.Id = A.ActivityType
           LEFT JOIN #STOCKQTY SQ 
               ON SQ.ItemCode = PSI.OldItemId 
               AND PSI.OldCategoryDescription = SQ.CategoryDescription 
               AND PSI.OldGroupName = SQ.GroupName
           WHERE PSH.IsDeleted = 0 AND PSD.IsActive=1
           {{(FromDueDate.HasValue ? "AND PSD.ActualWorkOrderDate >= @FromDueDate" : "")}}
            {{(ToDueDate.HasValue ? "AND PSD.ActualWorkOrderDate <= @ToDueDate" : "")}}
           GROUP BY 
               PSH.Id, 
               MM.MachineName, 
               MG.GroupName, 
               MISC.Description,
               A.ActivityName, 
               ActivityType.Code, 
               PSD.ActualWorkOrderDate,
               PSI.OldItemId, 
               PSI.OldCategoryDescription,
               SQ.UOM, 
               SQ.TotQty, 
               PSI.RequiredQty
           ORDER BY PSH.Id DESC;
        """;

            
              var parameters = new
                       {
                           FromDueDate = FromDueDate,
                           ToDueDate  = ToDueDate 
                       };

             var schedule = await _dbConnection.QueryMultipleAsync(query, parameters);
             var schedulelist = await schedule.ReadAsync<dynamic>();

            return schedulelist;
        }
    }
}
   