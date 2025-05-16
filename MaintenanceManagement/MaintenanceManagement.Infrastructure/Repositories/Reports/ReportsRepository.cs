using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IReports;
using Core.Application.Reports.MaintenanceRequestReport;
using Core.Application.Reports.WorkOrderReport;
using Core.Application.Reports.WorkOderCheckListReport;
using Core.Application.WorkOrder.Command.CreateWorkOrder;
using Dapper;
using MaintenanceManagement.Infrastructure.Repositories.Common;

namespace MaintenanceManagement.Infrastructure.Repositories.Reports
{
    public class ReportsRepository : BaseQueryRepository,IReportRepository
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
   