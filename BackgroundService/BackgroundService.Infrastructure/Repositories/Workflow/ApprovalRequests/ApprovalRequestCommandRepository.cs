using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Domain.Entities.Workflow;
using BackgroundService.Infrastructure.Data.Notification;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BackgroundService.Infrastructure.Repositories.Workflow.ApprovalRequests
{
    public class ApprovalRequestCommandRepository : IApprovalRequestCommand
    {
        private readonly NotificationDbContext _notificationDbContext;
        private readonly IDbConnection _dbConnection;
        public ApprovalRequestCommandRepository(NotificationDbContext notificationDbContext, IDbConnection dbConnection)
        {
            _notificationDbContext = notificationDbContext;
            _dbConnection = dbConnection;
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

        public async Task<bool> CreateBulkAsync(string workflowType, int transactionId, string contextJson)
        {
             var procParams = new
               {
                   WorkflowCode  = workflowType,
                   TransactionId = transactionId,             
                   ContextJson   = contextJson     
               };

                var affected = await _dbConnection.ExecuteAsync(
                  "[AppData].[sp_EvaluateApproval]",
                  procParams,
                  commandType: CommandType.StoredProcedure,
                  commandTimeout: 60
              );
          
             return affected > 0;

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