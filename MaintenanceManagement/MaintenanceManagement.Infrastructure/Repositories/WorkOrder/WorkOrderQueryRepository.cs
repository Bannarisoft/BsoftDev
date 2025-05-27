using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.Reports.WorkOrderItemConsuption;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Common;
using Dapper;
using MaintenanceManagement.Infrastructure.Repositories.Common;

namespace MaintenanceManagement.Infrastructure.Repositories.WorkOrder
{
    public class WorkOrderQueryRepository :BaseQueryRepository, IWorkOrderQueryRepository
    {
        private readonly IDbConnection _dbConnection;        
        public WorkOrderQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        : base(ipAddressService) 
        {
            _dbConnection = dbConnection;            
        }

        public async Task<List<WorkOrderWithScheduleDto>> GetAllWOAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate,int? requestTypeId, int? departmentId)
        {         
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);
            parameters.Add("@UnitId", UnitId);
            parameters.Add("@FromDate", fromDate);
            parameters.Add("@ToDate", toDate);
            parameters.Add("@RequestType", requestTypeId);
            parameters.Add("@DepartmentId", departmentId );

            List<WorkOrderWithScheduleDto> workOrderList;
            int totalCount;

            using (var multiResult = await _dbConnection.QueryMultipleAsync(
                "dbo.Usp_GetWorkOrder", parameters, commandType: CommandType.StoredProcedure))
            {
                workOrderList = (await multiResult.ReadAsync<WorkOrderWithScheduleDto>()).ToList();                
            }
            return (workOrderList);
        }

        public async Task<string> GetBaseDirectoryAsync()
        {
            const string query = @"
            SELECT Description AS BaseDirectory  
                FROM Maintenance.MiscTypeMaster 
                WHERE MiscTypeCode='WOImage'  
                AND IsDeleted=0
            ";
             var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(query);
            return result;               
        }          
      
        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWORootCauseDescAsync()
        {
             const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY SortOrder DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.WORootCause.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWOSourceDescAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY SortOrder DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.WOSource.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWOStoreTypeDescAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY SortOrder DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.GetWOStoreType.StoreType };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
        public async Task<List<Core.Domain.Entities.MiscMaster>> GetRequestTypeAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY SortOrder DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.GetRequestType.Code };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWOStatusDescAsync()
        {
           const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY SortOrder DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.WOStatus.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }             

        public async Task<(dynamic WorkOrderResult, IEnumerable<dynamic> Activity, IEnumerable<dynamic> Item, IEnumerable<dynamic> Technician, IEnumerable<dynamic> checkList, IEnumerable<dynamic> schedule)> GetWorkOrderByIdAsync(int workOrderId)         
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);
            parameters.Add("@UnitId", UnitId);
            parameters.Add("@WorkOrderId", workOrderId);

            using var multi = await _dbConnection.QueryMultipleAsync("dbo.Usp_GetWorkOrderById", parameters, commandType: CommandType.StoredProcedure);

            var workOrderResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var activity = (await multi.ReadAsync<dynamic>()) ?? Enumerable.Empty<dynamic>();
            var item = (await multi.ReadAsync<dynamic>()) ?? Enumerable.Empty<dynamic>();
            var technician = (await multi.ReadAsync<dynamic>()) ?? Enumerable.Empty<dynamic>();
            var checkList = (await multi.ReadAsync<dynamic>()) ?? Enumerable.Empty<dynamic>();
            var schedule = (await multi.ReadAsync<dynamic>()) ?? Enumerable.Empty<dynamic>();           
            
            return (workOrderResult, activity, item, technician, checkList, schedule);
        }

        public async Task<List<Core.Domain.Entities.WorkOrderMaster.WorkOrder>> GetWorkOrderAsync()
        {
            //var excludedStatusCode = MiscEnumEntity.MaintenanceStatusUpdate.Code;
          /*   var excludedStatusCodes = new[] 
            { 
                MiscEnumEntity.MaintenanceStatusUpdate.Code, 
                MiscEnumEntity.MaintenanceStatusCancelled.Code 
            }; */

           var excludedStatusCodes = new[] { "Closed", "Cancelled" };

            const string query = @"
                SELECT WO.Id, WO.WorkOrderDocNo
                FROM Maintenance.WorkOrder WO     
                INNER JOIN Maintenance.MiscMaster MM ON MM.ID = WO.StatusId   
                WHERE WO.CompanyId = @CompanyId 
                AND WO.UnitId = @UnitId  
                AND MM.Code NOT IN @ExcludedStatusCodes
                ORDER BY WO.Id";

            var parameters = new 
            { 
                CompanyId, 
                UnitId, 
                ExcludedStatusCodes = excludedStatusCodes // ✅ exact name match
            };

            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(query, parameters);
            return result.ToList();     
        }       
    } 
}
   