using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;
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

        public async Task<(List<GetWorkOrderByIdDto>, int)> GetAllWOAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var companyId = _ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@PageNumber", PageNumber );
            parameters.Add("@PageSize", PageSize );
            parameters.Add("@SearchTerm", SearchTerm);

                // ✅ Ensure using statement to properly handle GridReader disposal
            using var multiResult = await _dbConnection.QueryMultipleAsync(
                "dbo.GetWorkOrder", parameters, commandType: CommandType.StoredProcedure);

            // ✅ Read all data before exiting the using block
            var depreciationList = (await multiResult.ReadAsync<DepreciationDto>()).ToList();
            int totalCount = await multiResult.ReadFirstOrDefaultAsync<int>();

            return (depreciationList, totalCount); 
        }

        public Task<string> GetBaseDirectoryAsync()
        {
            throw new NotImplementedException();
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
                SELECT WorkOrderDocNo,WO.Remarks,MM1.Description+'/'+WO.Image Image,WO.StatusId,M.description StatusDesc,WO.RootCauseId,M1.description RootCauseDesc
                FROM Maintenance.WorkOrder WO
                INNER JOIN Maintenance.MiscMaster  M ON M.Id=WO.StatusId
                INNER JOIN Maintenance.MiscTypeMaster MM1 on MM1.MiscTypeCode ='WOImage'
                INNER JOIN Maintenance.MiscMaster  M1 ON M1.Id=WO.RootCauseId
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
    } 
}
   