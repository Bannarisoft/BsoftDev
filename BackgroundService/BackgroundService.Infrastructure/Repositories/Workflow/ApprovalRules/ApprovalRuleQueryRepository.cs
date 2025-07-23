using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRule;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Domain.Entities.Workflow;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Workflow.ApprovalRules
{
    public class ApprovalRuleQueryRepository : IApprovalRuleQuery
    {
        private readonly IDbConnection _dbConnection;
        public ApprovalRuleQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> AlreadyExistsAsync(string ConditionKey, string Operator, string Value, string Action, int UnitId, int WorkFlowTypeId, int? id = null)
        {
             var query = @"SELECT COUNT(1) FROM [AppData].[ApprovalRule] WHERE ConditionKey = @ConditionKey
             
             AND Operator = @Operator AND Value = @Value AND Action = @Action AND UnitId = @UnitId AND WorkflowTypeId = @WorkflowTypeId AND IsDeleted = 0";
            var parameters = new DynamicParameters(new { ConditionKey,Operator, Value,Action, UnitId, WorkFlowTypeId });

            if (id is not null)
            {
                query += " AND Id != @Id";
                parameters.Add("Id", id);
            }
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
            return count > 0;
        }

        public async Task<(List<ApprovalRule>, int)> GetAllApprovalRuleAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
              const string dataQuery =    @" SELECT 
                AR.Id, 
                AR.ConditionKey,
                AR.Operator,
                AR.Value,
                AR.Action,
                AR.UnitId,
                AR.WorkflowTypeId,
                AR.IsActive,AR.CreatedDate,AR.CreatedBy,AR.CreatedByName,AR.ModifiedBy,AR.ModifiedDate,AR.ModifiedByName,
                WT.Id,WT.ModuleTypeName
            FROM [AppData].[ApprovalRule] AR
            INNER JOIN [AppData].[WorkflowType] WorkFlow on WorkFlow.Id=ASD.WorkFlowTypeId
            WHERE 
            AR.IsDeleted = 0
                AND (@Search IS NULL OR WorkFlow.ModuleTypeName LIKE @Search)
                ORDER BY AR.Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";
              const string countQuery = @"
              SELECT COUNT(*) 
               FROM [AppData].[ApprovalRule] AR
            INNER JOIN [AppData].[WorkflowType] WorkFlow on WorkFlow.Id=ASD.WorkFlowTypeId
             WHERE AR.IsDeleted = 0  AND (@Search IS NULL OR WorkFlow.ModuleTypeName LIKE @Search );
          ";


            var parameters = new
            {
                Search = string.IsNullOrEmpty(SearchTerm) ? null : $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var ApprovalRule = await _dbConnection.QueryAsync<ApprovalRule, WorkflowType, ApprovalRule>(
                dataQuery,
                (approvalRule, workflow) =>
                {
                     approvalRule.WorkflowType = new WorkflowType
                     {
                         Id = workflow.Id,
                         ModuleTypeName = workflow.ModuleTypeName
                     };
                   
                     return approvalRule;
                },
                parameters,
                splitOn: "Id"                
                );
            
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);

            return (ApprovalRule.ToList(), totalCount);
        }

        public async Task<bool> NotFoundAsync(int id)
        {
             var query = "SELECT COUNT(1) FROM [AppData].[ApprovalRule]  WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
        }
    }
}