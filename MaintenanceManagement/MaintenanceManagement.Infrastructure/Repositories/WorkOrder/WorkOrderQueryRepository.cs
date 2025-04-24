using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;

// using Core.Application.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Common;
using Dapper;
using MassTransit;

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

        public Task<(List<GetWorkOrderByIdDto>, int)> GetAllWOAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            throw new NotImplementedException();
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

        Task<(List<GetWorkOrderByIdDto>, int)> IWorkOrderQueryRepository.GetAllWOAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            throw new NotImplementedException();
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
                FROM Maintenance.WorkOrder WO
                INNER JOIN Maintenance.WorkOrderActivity  WA ON WA.WorkOrderId=WO.Id
                INNER JOIN Maintenance.ActivityMaster  AM ON AM.ID=WA.ActivityId
                where WO.Id= @workOrderId;

                SELECT WI.StoreTypeId,M.description StoreTypeDesc,WI.ItemCode,WI.OldItemCode,WI.SourceId,M1.description SourceDesc,WI.ItemName,WI.AvailableQty,WI.UsedQty,WI.ScarpQty,WI.ToSubStoreQty,MM1.Description+'/'+WO.Image Image
                FROM Maintenance.WorkOrder WO
                INNER JOIN Maintenance.WorkOrderItem  WI ON WI.WorkOrderId=WO.Id
                LEFT JOIN Maintenance.MiscMaster  M ON M.Id=WI.StoreTypeId
                LEFT JOIN Maintenance.MiscMaster  M1 ON M1.Id=WI.SourceId
                INNER JOIN Maintenance.MiscTypeMaster MM1 on MM1.MiscTypeCode ='WOItemImage'
                where WO.Id=@workOrderId         
                
                SELECT WT.TechnicianId,WT.OldTechnicianId,WT.TechnicianName,WT.SourceId,M1.description SourceDesc,WT.HoursSpent,WT.MinutesSpent
                FROM Maintenance.WorkOrder WO
                INNER JOIN Maintenance.WorkOrderTechnician  WT ON WT.WorkOrderId=WO.Id
                LEFT JOIN Maintenance.MiscMaster  M1 ON M1.Id=WT.SourceId
                where WO.Id=@workOrderId 
                
                SELECT WC.CheckListId,AC.ActivityCheckList, WC.Description,WC.ISCompleted,WC.Description
                FROM Maintenance.WorkOrder WO
                INNER JOIN Maintenance.WorkOrderCheckList  WC ON WC.WorkOrderId=WO.Id
                INNER JOIN Maintenance.ActivityCheckListMaster  AC ON AC.Id=WC.CheckListId
                where WO.Id=@workOrderId

                SELECT WS.StartTime,WS.EndTime
                FROM Maintenance.WorkOrder WO
                INNER JOIN Maintenance.WorkOrderSchedule  WS ON WS.WorkOrderId=WO.Id
                where WO.Id=@workOrderId
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
   