using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Domain.Common;
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
        private readonly IIPAddressService _ipAddressService;
        
        public ApprovalRequestCommandRepository(NotificationDbContext notificationDbContext, IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _notificationDbContext = notificationDbContext;
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }

        public async Task<int> Approve(ApprovalRequest approvalRequest,CancellationToken ct)
        {
                var p = new DynamicParameters();
        p.Add("@HeaderId", approvalRequest.Id, DbType.Int32);
        p.Add("@JsonUpdates", approvalRequest.ApprovalRequestLines, DbType.String);
        p.Add("@Approved", MiscEnumEntity.Approved, DbType.Int32);
        p.Add("@Rejected", MiscEnumEntity.Rejected, DbType.Int32);
        p.Add("@Pending", MiscEnumEntity.Pending, DbType.Int32);
        p.Add("@ModifiedBy", _ipAddressService.GetUserId(), DbType.Int32);
        p.Add("@NewHeaderStatusId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        // Use CommandDefinition to pass the cancellation token
        var cmd = new CommandDefinition(
            commandText: "[AppData].[usp_Approval_UpdateLines]",
            parameters: p,
            commandType: CommandType.StoredProcedure,
            cancellationToken: ct
        );

         await _dbConnection.ExecuteAsync(cmd);

          return p.Get<int>("@NewHeaderStatusId");
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