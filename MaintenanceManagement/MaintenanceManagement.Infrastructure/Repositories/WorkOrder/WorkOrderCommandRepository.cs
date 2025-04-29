using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Common;
using Dapper;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Contracts.Events.Maintenance;
using MassTransit;

namespace MaintenanceManagement.Infrastructure.Repositories.WorkOrder
{
    public class WorkOrderCommandRepository: IWorkOrderCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;       
        private readonly IIPAddressService _ipAddressService; 
        private readonly IDbConnection _dbConnection;
        private readonly IPublishEndpoint _publishEndpoint;   
        public WorkOrderCommandRepository(ApplicationDbContext applicationDbContext, IIPAddressService ipAddressService,IDbConnection dbConnection, IPublishEndpoint publishEndpoint )
        {
            _applicationDbContext = applicationDbContext; 
            _ipAddressService = ipAddressService;     
            _dbConnection = dbConnection;     
            _publishEndpoint = publishEndpoint;
        }
        public async Task<Core.Domain.Entities.WorkOrderMaster.WorkOrder> CreateAsync(Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder, int requestTypeId, CancellationToken cancellationToken)
        {
            var entry =_applicationDbContext.Entry(workOrder);
            workOrder.WorkOrderDocNo = await GetLatestWorkOrderDocNo(requestTypeId);
            await _applicationDbContext.WorkOrder.AddAsync(workOrder);
            await _applicationDbContext.SaveChangesAsync();
            return workOrder;   
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
                "dbo.GetWorkOrderDocNo", 
                parameters, 
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120);
            return newAssetCode; 
        }
        public async Task<bool> UpdateAsync(int workOrderId,Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder)
        {
             var existingWorkOrder = await _applicationDbContext.WorkOrder
                   .Include(cf => cf.WorkOrderItems)
                   .Include(cf => cf.WorkOrderActivities)                   
                   .Include(cf => cf.WorkOrderTechnicians)
                   .Include(cf => cf.WorkOrderCheckLists)
                   .FirstOrDefaultAsync(u => u.Id ==workOrderId);

            if (existingWorkOrder == null)
                return false;
            
            _applicationDbContext.WorkOrderActivity.RemoveRange(
                _applicationDbContext.WorkOrderActivity.Where(x => x.WorkOrderId == workOrderId));

            _applicationDbContext.WorkOrderCheckList.RemoveRange(
                _applicationDbContext.WorkOrderCheckList.Where(x => x.WorkOrderId == workOrderId));

            _applicationDbContext.WorkOrderItem.RemoveRange(
                _applicationDbContext.WorkOrderItem.Where(x => x.WorkOrderId == workOrderId));                
        
            _applicationDbContext.WorkOrderTechnician.RemoveRange(
                _applicationDbContext.WorkOrderTechnician.Where(x => x.WorkOrderId == workOrderId));
        
            var createdBy = existingWorkOrder.CreatedBy;
            var createdByName = existingWorkOrder.CreatedByName;
            var createdIP  = existingWorkOrder.CreatedIP ;

                // Update scalar fields
            _applicationDbContext.Entry(existingWorkOrder).CurrentValues.SetValues(workOrder);
            existingWorkOrder.CreatedBy = createdBy;
            existingWorkOrder.CreatedByName = createdByName;
            existingWorkOrder.CreatedIP = createdIP;           

            await _applicationDbContext.AddRangeAsync(workOrder.WorkOrderActivities ?? []);
            await _applicationDbContext.AddRangeAsync(workOrder.WorkOrderItems ?? []);
            await _applicationDbContext.AddRangeAsync(workOrder.WorkOrderTechnicians ?? []);
            await _applicationDbContext.AddRangeAsync(workOrder.WorkOrderCheckLists ?? []);   
            var result= await _applicationDbContext.SaveChangesAsync();         

            int docSerialNumber = 1;
            foreach (var item in workOrder.WorkOrderItems ?? [])
            {
                if ((item.UsedQty > 0) || (item.ToSubStoreQty > 0))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@OldUnitCode", "01");                                        
                    parameters.Add("@DocNo", workOrder.Id);
                    parameters.Add("@DocSNo", docSerialNumber);                    
                    parameters.Add("@ItemCode", item.OldItemCode);
                    parameters.Add("@ItemName", item.ItemName);                    
                    parameters.Add("@UsedQty", item.UsedQty);                    
                    parameters.Add("@SubStoreQty", item.ToSubStoreQty);                                       

                    await _dbConnection.ExecuteAsync(
                        "usp_InsertStockLedger",  // your stored procedure name
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );                     
                }
                string tempItemFilePath = item.Image;
                if (tempItemFilePath != null){
                    string baseDirectory =await GetBaseDirectoryItemAsync();

                    var (companyName, unitName) = await GetCompanyUnitAsync(workOrder.CompanyId, workOrder.UnitId);

                    string companyFolder = Path.Combine(baseDirectory, companyName.Trim());
                    string unitFolder = Path.Combine(companyFolder,unitName.Trim());
                    string filePath = Path.Combine(unitFolder, tempItemFilePath);                

                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                        string newFileName = $"{workOrder.WorkOrderDocNo}-{docSerialNumber}{Path.GetExtension(tempItemFilePath)}";
                        string newFilePath = Path.Combine(directory, newFileName);

                        try
                        {
                            File.Move(filePath, newFilePath);
                            //assetEntity.AssetImage = newFileName;
                            await UpdateWOItemImageAsync(item.Id, newFileName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to rename file: {ex.Message}");
                        }
                    }
                }
                docSerialNumber++;
            }       
            // ✅ Update TotalManPower and TotalSpentHours if status is "Closed"
            var closedStatusId = await _applicationDbContext.MiscMaster
                .Where(x => x.Code == MiscEnumEntity.MaintenanceStatusUpdate.Code)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (workOrder.StatusId == closedStatusId)
            {
                var technicianCount = workOrder.WorkOrderTechnicians?.Count ?? 0;
                var totalHours = workOrder.WorkOrderTechnicians?.Sum(t => t.HoursSpent + (t.MinutesSpent / 60.0)) ?? 0;

                existingWorkOrder.TotalManPower = technicianCount;
                existingWorkOrder.TotalSpentHours = (decimal?)Math.Round(totalHours, 2);

                  // 🔥 Publish event for next scheduler creation
               /*  if (workOrder.PreventiveScheduleId.HasValue)
                {
                    var correlationId = Guid.NewGuid();
                    await  _publishEndpoint.Publish (new WorkOrderClosedEvent
                    {
                        CorrelationId = correlationId, 
                        PreventiveSchedulerDetailId =workOrder.PreventiveScheduleId.Value,
                        WorkOrderId = workOrder.Id
                    });
                }                 */
            }               

            return result> 0;
        }
        public async Task<string> GetBaseDirectoryItemAsync()
        {
            const string query = @"
            SELECT Description AS BaseDirectory  
                FROM Maintenance.MiscTypeMaster 
                WHERE MiscTypeCode='WOItemImage'  
                AND IsDeleted=0
            ";
             var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(query);
            return result;               
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
        public async Task<bool> RemoveWOImageReferenceAsync(int workOrderId)
        {
            var asset = await _applicationDbContext.WorkOrder.FindAsync(workOrderId);
            if (asset == null)
            {
                return false;  // Asset not found
            }

            asset.Image = null;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> CreateScheduleAsync(int workOrderId, Core.Domain.Entities.WorkOrderMaster.WorkOrderSchedule workOrderSchedule)
        {       
            await _applicationDbContext.WorkOrderSchedule.AddAsync(workOrderSchedule);
            await _applicationDbContext.SaveChangesAsync();

            // Check if this is the only schedule for the given WorkOrderId
            var existingScheduleCount = await _applicationDbContext.WorkOrderSchedule
                .CountAsync(ws => ws.WorkOrderId == workOrderId);           
            // If it's the first schedule, update MaintenanceRequest status
            if (existingScheduleCount == 1)
            {
                var workOrder  = await _applicationDbContext.WorkOrder
                .FirstOrDefaultAsync(wo  => wo.Id == workOrderId);

                var status  = await _applicationDbContext.MiscMaster
                .FirstOrDefaultAsync(mm => mm.Code == MiscEnumEntity.GetStatusId.Status);

                var maintenanceRequest = await _applicationDbContext.MaintenanceRequest
                    .FirstOrDefaultAsync(mr => mr.Id == workOrder.RequestId);
                if (maintenanceRequest != null)
                {
                    maintenanceRequest.RequestStatusId =status.Id; // Start work
                    _applicationDbContext.MaintenanceRequest.Update(maintenanceRequest);
                    await _applicationDbContext.SaveChangesAsync();
                }
            }
            return workOrderSchedule.Id;                       
        }
        public async Task<bool> UpdateScheduleAsync(int workOrderId, Core.Domain.Entities.WorkOrderMaster.WorkOrderSchedule workOrderSchedule)
        {           
            //var existingWO =await _applicationDbContext.WorkOrderSchedule.FirstOrDefaultAsync(m =>m.WorkOrderId == workOrderId);
            var existingWO = await _applicationDbContext.WorkOrderSchedule
                .Where(m => m.WorkOrderId == workOrderId)
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();
            if (existingWO != null)
            {
                existingWO.EndTime = workOrderSchedule.EndTime;
                existingWO.ISCompleted=workOrderSchedule.ISCompleted;
                _applicationDbContext.WorkOrderSchedule.Update(existingWO);
                return await _applicationDbContext.SaveChangesAsync() > 0;
            }
           return false;                    
        }
        public async Task<Core.Domain.Entities.WorkOrderMaster.WorkOrder> GetByIdAsync(int workOrderId)
        {
           return await _applicationDbContext.WorkOrder
                     .FirstOrDefaultAsync(x => x.Id == workOrderId);
        }
        public async Task<bool> UpdateWOImageAsync(int workOrderId, string imageName)
        {
            var workOrder = await _applicationDbContext.WorkOrder.FindAsync(workOrderId);
            if (workOrder == null)
            {
                return false;  
            }          
            workOrder.Image = imageName;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteWOImageAsync(string imageName)
        {
            var workOrder = await _applicationDbContext.WorkOrder.FirstOrDefaultAsync(x => x.Image == imageName);
            if (workOrder == null)
            {
                return false;  
            }          
            workOrder.Image = "";
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateWOItemImageAsync(int workOrderId, string imageName)
        {
            var workOrder = await _applicationDbContext.WorkOrderItem.FindAsync(workOrderId);
            if (workOrder == null)
            {
                return false;  
            }          
            workOrder.Image = imageName;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteItemImageAsync(string imageName)
        {
            var workOrder = await _applicationDbContext.WorkOrderItem.FirstOrDefaultAsync(x => x.Image == imageName);
            if (workOrder == null)
            {
                return false;  
            }          
            workOrder.Image = "";
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Core.Domain.Entities.MiscMaster?> GetMiscMasterByCodeAsync(string code)
        {
            return await _applicationDbContext.MiscMaster
                .FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}