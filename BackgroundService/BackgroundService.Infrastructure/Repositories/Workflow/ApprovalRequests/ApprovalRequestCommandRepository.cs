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

        public async Task<bool> Approve(ApprovalRequest approvalRequest,CancellationToken ct)
        {
              var header = await _notificationDbContext.ApprovalRequest
        .Include(h => h.ApprovalRequestLines)
        .FirstOrDefaultAsync(h => h.Id == approvalRequest.Id, ct);

           if (header == null) return false;
        
           using var tx = await _notificationDbContext.Database.BeginTransactionAsync(ct);
        
           // 2) Header: copy exactly from payload (status is authoritative from request)
           header.StatusId        = approvalRequest.StatusId;
           header.ModifiedBy      = approvalRequest.ModifiedBy;
           header.ModifiedByName  = approvalRequest.ModifiedByName;
           header.ModifiedDate    = approvalRequest.ModifiedDate;
           header.ModifiedIP      = approvalRequest.ModifiedIP;
        
           // 3) Lines: update only those present in the payload, by Id
           if (approvalRequest.ApprovalRequestLines != null && approvalRequest.ApprovalRequestLines.Count > 0)
           {
               var existingById = header.ApprovalRequestLines.ToDictionary(l => l.Id);
        
               foreach (var patch in approvalRequest.ApprovalRequestLines)
               {
                   if (!existingById.TryGetValue(patch.Id, out var line))
                   {
                       // Optional: support adds when Id == 0
                       // if (patch.Id == 0) { header.ApprovalRequestLines.Add(patch); continue; }
                       continue; // ignore unknown Ids
                   }
        
                   // Apply EXACTLY what client sent (no extra logic)
                   line.StatusId       = patch.StatusId;
        
                   // Audit from header payload (or use patch fields if you prefer)
                   line.ModifiedBy     = approvalRequest.ModifiedBy;
                   line.ModifiedByName = approvalRequest.ModifiedByName;
                   line.ModifiedDate   = approvalRequest.ModifiedDate;
                   line.ModifiedIP     = approvalRequest.ModifiedIP;
               }
           }
        
           var rows = await _notificationDbContext.SaveChangesAsync(ct);
           await tx.CommitAsync(ct);
           return rows > 0;
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