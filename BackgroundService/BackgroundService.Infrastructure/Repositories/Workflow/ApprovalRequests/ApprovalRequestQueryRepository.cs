using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Workflow.ApprovalRequests
{
    public class ApprovalRequestQueryRepository : IApprovalRequestQuery
    {
        private readonly IDbConnection _dbConnection;
        public ApprovalRequestQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<int> GetApprovalStepDetailByIdAsync(int WorkFlowTypeId, int ModuleTransactionId)
        {
            const string query = @"
                SELECT TOP 1 ASD.Id
            FROM [AppData].[ApprovalStepDetail] ASD
            LEFT JOIN [AppData].[ApprovalRequest] AR 
                ON AR.ApprovalStepDetailId = ASD.Id 
                AND AR.WorkflowTypeId = @WorkFlowTypeId 
                AND AR.ModuleTransactionId = @ModuleTransactionId
            WHERE ASD.IsDeleted = 0 
              AND ASD.IsActive = 1 
              AND AR.Id IS NULL 
            ORDER BY ASD.StepOrder ASC;";
                
            var WorkflowType = await _dbConnection.QueryAsync<int>(query, new { WorkFlowTypeId, ModuleTransactionId });
            return WorkflowType.FirstOrDefault();
        }
    }
}