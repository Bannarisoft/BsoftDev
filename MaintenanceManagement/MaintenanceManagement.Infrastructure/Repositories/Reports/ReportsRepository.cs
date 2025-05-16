using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IReports;
using Core.Application.Reports.GetStockLegerReport;
using Core.Application.Reports.MaintenanceRequestReport;
using Core.Application.Reports.WorkOrderItemConsuption;
using Core.Application.Reports.WorkOrderReport;
using Core.Application.StockLedger.Queries.GetCurrentStock;
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

        public async Task<List<RequestReportDto>> MaintenanceReportAsync(DateTimeOffset? requestFromDate, DateTimeOffset? requestToDate, int? RequestType, int? requestStatus, int? departmentId)
        {

            var query = @"
            SELECT 
                a.Id AS RequestId,
                a.UnitId,
                A.RequestTypeId,
                M.Code AS RequestType,
                a.CreatedDate AS RequestCreatedDate,
                a.CreatedBy,
                d.FirstName AS RequestCreatedName,
                a.DepartmentId,
                G.DeptName as Department,
                a.MachineId,
                E.MachineName,
                a.MaintenanceTypeId,
                F.Code as MaintenanceType,
                b.Id AS WorkOrderId,
                b.StatusId,
                H.Code as RequestStatus,
                a.ModifiedDate,
                A.OldVendorId,
                A.OldVendorName,
                A.ModeOfDispatchId,
                L.Code as ModeOfDispatch,
                A.ExpectedDispatchDate,
                A.EstimatedSpareCost,
                A.EstimatedServiceCost,
                A.ServiceLocationId,
                I.Code AS ServiceLocation,
                A.ServiceTypeId,
                J.code AS ServiceType,
                A.SparesTypeId,
                K.Code  AS SparesType,
                A.ModeOfDispatchId,
                L.Code AS DispatchMode,
                DATEDIFF(MINUTE, a.CreatedDate, a.ModifiedDate) AS RequestMinutesDifference,
                RIGHT('0' + CAST(DATEDIFF(MINUTE, a.CreatedDate, a.ModifiedDate) / 60 AS VARCHAR), 2) + ':' +
                RIGHT('0' + CAST(DATEDIFF(MINUTE, a.CreatedDate, a.ModifiedDate) % 60 AS VARCHAR), 2) AS DownTime,
                RIGHT('0' + CAST(SUM(DATEDIFF(SECOND, c.StartTime, c.EndTime)) / 3600 AS VARCHAR), 2) + ':' +
                RIGHT('0' + CAST((SUM(DATEDIFF(SECOND, c.StartTime, c.EndTime) % 3600) / 60) AS VARCHAR), 2) AS TimeTakenToRepair
            FROM Maintenance.MaintenanceRequest a
                    LEFT JOIN Maintenance.WorkOrder b ON a.Id = b.RequestId
                    LEFT JOIN Maintenance.WorkOrderSchedule c ON c.WorkOrderId = b.Id
                    LEFT JOIN BANNARI.AppSecurity.Users d ON a.CreatedBy = d.UserId                        
                    LEFT JOIN  Maintenance.MachineMaster E  ON A.MachineId=E.Id 
                    LEFT JOIN  Maintenance.MiscMaster F  ON A.MaintenanceTypeId=F.Id 
                    LEFT JOIN  Bannari.AppData.Department G  ON A.DepartmentId=G.Id
                    LEFT JOIN  Maintenance.MiscMaster H  ON A.RequestStatusId=H.Id
                    LEFT JOIN  Maintenance.MiscMaster I  ON A.ServiceLocationId=I.Id
                    LEFT JOIN  Maintenance.MiscMaster J  ON A.ServiceTypeId=J.Id
                    LEFT JOIN  Maintenance.MiscMaster K  ON A.SparesTypeId=K.Id
                    LEFT JOIN  Maintenance.MiscMaster L  ON A.ModeOfDispatchId=L.Id
                    LEFT JOIN  Maintenance.MiscMaster M  ON A.RequestTypeId=M.Id
            WHERE a.CreatedDate >= @RequestFromDate
                    AND a.CreatedDate <= @RequestToDate
                    AND (@RequestType IS NULL OR  @RequestType ='' OR a.RequestTypeId = @RequestType)
                    AND (@RequestStatus IS NULL OR  @RequestStatus ='' OR a.RequestStatusId = @RequestStatus)
                    AND (@DepartmentId IS NULL OR   @DepartmentId ='' OR a.DepartmentId = @DepartmentId)                                                
            GROUP BY 
                    a.Id, a.UnitId, a.CreatedDate, a.CreatedBy, a.DepartmentId,
                    a.MachineId, a.MaintenanceTypeId, b.Id, b.StatusId, a.ModifiedDate,
                    d.FirstName, G.DeptName, E.MachineName, F.Code, H.Code,
                    a.OldVendorId, a.OldVendorName, a.ModeOfDispatchId, a.ExpectedDispatchDate,
                    a.EstimatedSpareCost, a.EstimatedServiceCost,
                    a.ServiceLocationId, I.Code, a.ServiceTypeId, J.Code,
                    a.SparesTypeId, K.Code,L.Code,M.Code,A.RequestTypeId
            ORDER BY a.Id";

            var parameters = new
            {
                RequestFromDate = requestFromDate?.Date,
                RequestToDate = requestToDate?.Date.AddDays(1).AddTicks(-1), // To include full day
                RequestType = RequestType,
                RequestStatus = requestStatus,
                DepartmentId = departmentId
            };

            var result = await _dbConnection.QueryAsync<RequestReportDto>(query, parameters);
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
    }
}
   