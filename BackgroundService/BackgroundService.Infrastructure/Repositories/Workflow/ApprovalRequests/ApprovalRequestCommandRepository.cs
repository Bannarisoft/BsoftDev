using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Domain.Entities.Workflow;
using BackgroundService.Infrastructure.Data.Notification;

namespace BackgroundService.Infrastructure.Repositories.Workflow.ApprovalRequests
{
    public class ApprovalRequestCommandRepository : IApprovalRequestCommand
    {
        private readonly NotificationDbContext _notificationDbContext;
        public ApprovalRequestCommandRepository(NotificationDbContext notificationDbContext)
        {
            _notificationDbContext = notificationDbContext;
        }
        public async Task<int> CreateAsync(ApprovalRequest approvalRequest)
        {
             _notificationDbContext.Entry(approvalRequest);
            await _notificationDbContext.ApprovalRequest.AddAsync(approvalRequest);
            await _notificationDbContext.SaveChangesAsync();

            return approvalRequest.Id;
        }
    }
}