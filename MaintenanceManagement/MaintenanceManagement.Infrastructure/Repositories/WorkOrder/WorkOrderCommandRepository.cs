using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Common;
using Dapper;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.Infrastructure.Repositories.WorkOrder
{
    public class WorkOrderCommandRepository: IWorkOrderCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;       
        private readonly IIPAddressService _ipAddressService; 
        private readonly IDbConnection _dbConnection;
        public WorkOrderCommandRepository(ApplicationDbContext applicationDbContext, IIPAddressService ipAddressService,IDbConnection dbConnection )
        {
            _applicationDbContext = applicationDbContext; 
            _ipAddressService = ipAddressService;     
            _dbConnection = dbConnection;     
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
                "dbo.FAM_GetWorkOrderDocNo", 
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
            }

            await _applicationDbContext.AddRangeAsync(workOrder.WorkOrderActivities ?? []);
            await _applicationDbContext.AddRangeAsync(workOrder.WorkOrderItems ?? []);
            await _applicationDbContext.AddRangeAsync(workOrder.WorkOrderTechnicians ?? []);
            await _applicationDbContext.AddRangeAsync(workOrder.WorkOrderCheckLists ?? []);               
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }


        public async Task<bool> UpdateWOImageAsync(int workOrderId, string imageName)
        {
            var imageExists = await _applicationDbContext.WorkOrder.FindAsync(workOrderId);
            if (imageExists == null)
            {
                return false; 
            }            
            imageExists.Image = imageName.Replace(@"\", "/"); 

            imageExists.Image = imageName;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        // public async Task<WorkOrderDto?> GetByWOImageAsync(int workOrderId)
        // {
        //     return await _applicationDbContext.WorkOrder
        //     .Where(a => a.Id == workOrderId)
        //     .Select(a => new WorkOrderDto
        //     {
        //         Id = a.Id,
        //         Image = a.Image                
        //     })
        //     .FirstOrDefaultAsync();
        // }

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

        public async Task<bool> UpdateAssetImageAsync(int WoId, string imageName)
        {
            var workOrder = await _applicationDbContext.WorkOrder.FindAsync(WoId);
            if (workOrder == null)
            {
                return false;  
            }          
            workOrder.Image = imageName;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }
    }
}