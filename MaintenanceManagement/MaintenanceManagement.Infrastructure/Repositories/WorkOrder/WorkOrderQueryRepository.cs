using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Common;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.WorkOrder
{
    public class WorkOrderQueryRepository : IWorkOrderQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
        public WorkOrderQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }

        public async Task<(List<WorkOrderWithScheduleDto>, int)> GetAllWOAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate,string? requestType, int PageNumber, int PageSize, string? SearchTerm)
        {
            var companyId = _ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@FromDate", fromDate);
            parameters.Add("@ToDate", toDate);
            parameters.Add("@RequestType", requestType);
            parameters.Add("@PageNumber", PageNumber );
            parameters.Add("@PageSize", PageSize );
            parameters.Add("@SearchTerm", SearchTerm);

            List<WorkOrderWithScheduleDto> workOrderList;
            int totalCount;

            using (var multiResult = await _dbConnection.QueryMultipleAsync(
                "dbo.GetWorkOrder", parameters, commandType: CommandType.StoredProcedure))
            {
                workOrderList = (await multiResult.ReadAsync<WorkOrderWithScheduleDto>()).ToList();
                totalCount = await multiResult.ReadFirstOrDefaultAsync<int>();
            }
            return (workOrderList, totalCount);
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

        public async Task<string?> GetLatestWorkOrderDocNo(int TypeId)
        {
            var companyId = _ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@TypeId", TypeId);
            var newAssetCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "dbo.FAM_GetWorkOrderDocNo", 
                parameters, 
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120);
            return newAssetCode; 
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
            var sqlQuery = @"
                -- First Query: AssetMaster (One-to-One)
                SELECT WorkOrderDocNo,WO.Remarks,MM1.Description+'/'+WO.Image Image,WO.StatusId,M.description StatusDesc,WO.RootCauseId,M1.description RootCauseDesc,
                WO.CreatedDate,WO.DownTimeStart,WO.DownTimeEnd,case when isnull(requestid,0)<>0 then MA.MachineCode else MA1.MachineCode end  Machine,
                case when isnull(requestid,0)<>0 then D.DeptName else D1.DeptName end Department,
                case when isnull(requestid,0)<>0 then  requestid else PreventiveScheduleId end RequestId
                FROM Maintenance.WorkOrder WO
                INNER JOIN Maintenance.MiscMaster  M ON M.Id=WO.StatusId
                INNER JOIN Maintenance.MiscTypeMaster MM1 on MM1.MiscTypeCode ='WOImage'
                INNER JOIN Maintenance.MiscMaster  M1 ON M1.Id=WO.RootCauseId
                LEFT JOIN [Maintenance].[MaintenanceRequest]  MR on MR.ID=WO.RequestId
                LEFT JOIN [Maintenance].[PreventiveSchedulerDetail]  PS on PS.ID=WO.PreventiveScheduleId
                LEFT JOIN [Maintenance].[PreventiveSchedulerHeader] PH on PH.Id=PS.PreventiveSchedulerId
                LEFT JOIN [Maintenance].[MachineMaster] MA on MA.ID=MR.MachineId
                LEFT JOIN [Maintenance].[MachineMaster] MA1 on MA1.ID=PS.MachineId
                LEFT JOIN Bannari.AppData.Department D on D.Id=MR.DepartmentId
                LEFT JOIN Bannari.AppData.Department D1 on D1.Id=PH.DepartmentId
                where WO.Id= @workOrderId;

                SELECT WA.ActivityId,AM.ActivityName,WA.Description
                FROM Maintenance.WorkOrderActivity  WA 
                INNER JOIN Maintenance.ActivityMaster  AM ON AM.ID=WA.ActivityId
                where WA.WorkOrderId= @workOrderId;

                SELECT WI.StoreTypeId,M.description StoreTypeDesc,WI.ItemCode,WI.OldItemCode,WI.SourceId,M1.description SourceDesc,WI.ItemName,WI.AvailableQty,WI.UsedQty,WI.ScarpQty,WI.ToSubStoreQty,MM1.Description+'/'+WI.Image Image
                FROM  Maintenance.WorkOrderItem  WI 
                LEFT JOIN Maintenance.MiscMaster  M ON M.Id=WI.StoreTypeId
                LEFT JOIN Maintenance.MiscMaster  M1 ON M1.Id=WI.SourceId
                INNER JOIN Maintenance.MiscTypeMaster MM1 on MM1.MiscTypeCode ='WOItemImage'
                where WI.WorkOrderId=@workOrderId         
                
                SELECT WT.TechnicianId,WT.OldTechnicianId,WT.TechnicianName,WT.SourceId,M1.description SourceDesc,WT.HoursSpent,WT.MinutesSpent
                FROM  Maintenance.WorkOrderTechnician  WT 
                LEFT JOIN Maintenance.MiscMaster  M1 ON M1.Id=WT.SourceId
                where WT.WorkOrderId=@workOrderId 
                
                SELECT WC.CheckListId,AC.ActivityCheckList, WC.Description,WC.ISCompleted,WC.Description
                FROM  Maintenance.WorkOrderCheckList  WC 
                INNER JOIN Maintenance.ActivityCheckListMaster  AC ON AC.Id=WC.CheckListId
                where WC.WorkOrderId=@workOrderId

                SELECT WS.StartTime,WS.EndTime
                FROM  Maintenance.WorkOrderSchedule  WS
                where WS.WorkOrderId=@workOrderId
            ";

            using var multi = await _dbConnection.QueryMultipleAsync(sqlQuery, new { WorkOrderId = workOrderId });

            var WorkOrderResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var Activity = await multi.ReadAsync<dynamic>();
            var Item = await multi.ReadAsync<dynamic>();                       
            var Technician = await multi.ReadAsync<dynamic>();                        
            var checkList = await multi.ReadAsync<dynamic>(); 
            var Schedule = await multi.ReadAsync<dynamic>(); 
           
            return (WorkOrderResult, Activity,  Item, Technician,checkList,Schedule);
        }

        public async Task<(string CompanyName, string UnitName)> GetCompanyUnitAsync(int companyId,int unitId)
        {
            const string query = @"
                SELECT CompanyName 
                FROM Bannari.AppData.Company 
                WHERE Id = @CompanyId;

                SELECT UnitName  
                FROM Bannari.AppData.Unit 
                WHERE Id = @UnitId;
            ";
            using var multiQuery = await _dbConnection.QueryMultipleAsync(query, new { CompanyId = companyId, UnitId = unitId });

            var companyName = await multiQuery.ReadFirstOrDefaultAsync<string>();
            var unitName = await multiQuery.ReadFirstOrDefaultAsync<string>();

            return (companyName ?? "Unknown Company", unitName ?? "Unknown Unit");


        }
    } 
}
   