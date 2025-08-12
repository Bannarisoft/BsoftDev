using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Domain.Entities.Workflow;
using BackgroundService.Infrastructure.Data.Notification;
using Microsoft.EntityFrameworkCore;

namespace BackgroundService.Infrastructure.Repositories.Workflow.ApprovalRequests
{
    public class ApprovalRequestCommandRepository : IApprovalRequestCommand
    {
        private readonly NotificationDbContext _notificationDbContext;
        public ApprovalRequestCommandRepository(NotificationDbContext notificationDbContext)
        {
            _notificationDbContext = notificationDbContext;
        }

        public async Task<bool> Approve(ApprovalRequest approvalRequest)
        {
            var existingApprovalReq = await _notificationDbContext.ApprovalRequest
            .AsNoTracking().FirstOrDefaultAsync(u => u.Id == approvalRequest.Id);
            
            if (existingApprovalReq != null)
            {
                existingApprovalReq.StatusId = approvalRequest.StatusId;
                existingApprovalReq.ModifiedBy = approvalRequest.ModifiedBy;
                existingApprovalReq.ModifiedByName = approvalRequest.ModifiedByName;
                existingApprovalReq.ModifiedDate = approvalRequest.ModifiedDate;
                existingApprovalReq.ModifiedIP = approvalRequest.ModifiedIP;
                _notificationDbContext.ApprovalRequest.Update(existingApprovalReq);

                return await _notificationDbContext.SaveChangesAsync() >0;
            }
            
            return false; 
        }

        public async Task<bool> CreateBulkAsync(List<ApprovalRequest> approvalRequest)
        {
             _notificationDbContext.Entry(approvalRequest);
            await _notificationDbContext.ApprovalRequest.AddRangeAsync(approvalRequest);
           return await _notificationDbContext.SaveChangesAsync() > 0;

        }

        public async Task<bool> Reject(ApprovalRequest approvalRequest)
        {
            var existingApprovalReq = await _notificationDbContext.ApprovalRequest
            .AsNoTracking().FirstOrDefaultAsync(u => u.Id == approvalRequest.Id);
            
            if (existingApprovalReq != null)
            {
                existingApprovalReq.StatusId = approvalRequest.StatusId;
                existingApprovalReq.ModifiedBy = approvalRequest.ModifiedBy;
                existingApprovalReq.ModifiedByName = approvalRequest.ModifiedByName;
                existingApprovalReq.ModifiedDate = approvalRequest.ModifiedDate;
                existingApprovalReq.ModifiedIP = approvalRequest.ModifiedIP;
                _notificationDbContext.ApprovalRequest.Update(existingApprovalReq);

                return await _notificationDbContext.SaveChangesAsync() >0;
            }
            
            return false; 
        }
    }
}