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

        public async Task<List<WorkOrderIssueDto>> GetItemConsumptionAsync(DateTimeOffset IssueFromDate, DateTimeOffset IssueToDate, int maintenanceTypeId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", IssueFromDate);
            parameters.Add("@ToDate", IssueToDate);
            parameters.Add("@UnitId", UnitId);
            parameters.Add("@MaintenanceTypeId", maintenanceTypeId);

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
    }
}
   