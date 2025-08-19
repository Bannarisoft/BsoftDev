using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.Interfaces;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalRequest;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Domain.Entities.Workflow;
using Dapper;

namespace BackgroundService.Infrastructure.Repositories.Workflow.ApprovalRequests
{
    public class ApprovalRequestQueryRepository : IApprovalRequestQuery
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipaddressService;
        public ApprovalRequestQueryRepository(IDbConnection dbConnection, IIPAddressService ipaddressService)
        {
            _dbConnection = dbConnection;
            _ipaddressService = ipaddressService;
        }

        public async Task<(List<ApprovalRequest>, int)> GetAllApprovalRequestAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            const string dataQuery = @" SELECT 
                AR.Id, 
                AR.WorkFlowTypeId,
                AR.ModuleTransactionId,
                AR.ApprovalStepDetailId,
                AR.ApprovalRuleId,
                AR.StatusId,
                AR.RequestedDate,
                ASD.TargetTypeId,
                Status.Code,WorkFlow.ModuleTypeName
            FROM [AppData].[ApprovalRequest] AR
            INNER JOIN [AppData].[ApprovalStepDetail] ASD on ASD.Id=AR.ApprovalStepDetailId
            INNER JOIN [AppData].[MiscMaster] Status on Status.Id=AR.StatusId
            INNER JOIN [AppData].[WorkflowType] WorkFlow on WorkFlow.Id=AR.WorkflowTypeId
            INNER JOIN [AppData].[ApprovalStepUnitMapping] ASM on ASM.ApprovalStepDetailId=ASD.Id
            WHERE 
             (@Search IS NULL OR Status.Code LIKE @Search OR WorkFlow.ModuleTypeName LIKE @Search)
                AND ASD.TargetTypeId= @Userid AND ASM.UnitId= @UnitId
                ORDER BY AR.Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";
            const string countQuery = @"
              SELECT COUNT(*) 
               FROM [AppData].[ApprovalRequest] AR
            INNER JOIN [AppData].[ApprovalStepDetail] ASD on ASD.Id=AR.ApprovalStepDetailId
            INNER JOIN [AppData].[MiscMaster] Status on Status.Id=AR.StatusId
            INNER JOIN [AppData].[WorkflowType] WorkFlow on WorkFlow.Id=AR.WorkflowTypeId
            INNER JOIN [AppData].[ApprovalStepUnitMapping] ASM on ASM.ApprovalStepDetailId=ASD.Id
            WHERE 
             (@Search IS NULL OR Status.Code LIKE @Search OR WorkFlow.ModuleTypeName LIKE @Search)
                AND ASD.TargetTypeId= @Userid AND ASM.UnitId= @UnitId;
          ";


            var parameters = new
            {
                Search = string.IsNullOrEmpty(SearchTerm) ? null : $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize,
                Userid = _ipaddressService.GetUserId(),
                UnitId = _ipaddressService.GetUnitId()
            };

            var ApprovalRequest = await _dbConnection.QueryAsync<ApprovalRequest, ApprovalStepDetail, Domain.Entities.Notification.MiscMaster, ApprovalRequest>(
                dataQuery,
                (approvalReq, detail, status) =>
                {
                    // approvalReq.ApprovalStepDetail = new ApprovalStepDetail
                    // {
                    //     TargetTypeId = detail.TargetTypeId
                    // };
                    approvalReq.Status = new Domain.Entities.Notification.MiscMaster
                    {
                        Code = status.Code
                    };
                    // approvalReq.WorkflowType = new WorkflowType
                    // {
                    //     ModuleTypeName = workFlow.ModuleTypeName
                    // };
                    return approvalReq;
                },
                parameters,
                splitOn: "TargetTypeId,Code"
                );

            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);

            return (ApprovalRequest.ToList(), totalCount);
        }

        public async Task<List<int>> GetAllApprovalRequestByApproved(string ModuleTypeName)
        {
            const string dataQuery = @" 
               SELECT 
                    AR.ModuleTransactionId
                    FROM [AppData].[ApprovalRequest] AR
                    INNER JOIN [AppData].[WorkflowType] WF ON WF.Id = AR.WorkflowTypeId
                    INNER JOIN [AppData].[MiscMaster] Status ON Status.Id = AR.StatusId
                    WHERE WF.ModuleTypeName = @ModuleTypeName
                    GROUP BY AR.ModuleTransactionId
                 HAVING COUNT(CASE WHEN Status.Code = 'Approved' THEN 1 END) = COUNT(*)
            ";

            var parameters = new
            {
                ModuleTypeName,
                Status = MiscEnumEntity.Pending
            };
            var result = await _dbConnection.QueryAsync<int>(dataQuery, parameters);
            return result.ToList();
        }

        public async Task<List<dynamic>> GetAllApprovalRequestByApprover(string ModuleTypeName, int ApproverId)
        {
            const string dataQuery = @" 
                SELECT 
                    AR.ModuleTransactionId,
                    AR.Id AS ApprovalRequestId,
                    Status.Code AS CurrentStatus,
                    WorkFlow.ModuleTypeName,
                    ApprovalStep.Code AS ApprovalStep,
                    ASD.StepOrder
                    
                FROM [AppData].[ApprovalRequest] AR
                INNER JOIN [AppData].[ApprovalStepDetail] ASD ON ASD.Id = AR.ApprovalStepDetailId
                INNER JOIN [AppData].[MiscMaster] Status ON Status.Id = AR.StatusId
                INNER JOIN [AppData].[WorkflowType] WorkFlow ON WorkFlow.Id = AR.WorkflowTypeId
                INNER JOIN [AppData].[MiscMaster] ApprovalStep ON ApprovalStep.Id = ASD.ApprovalStepId
                WHERE WorkFlow.ModuleTypeName = @ModuleTypeName AND ASD.TargetTypeId= @ApproverId
                AND Status.Code=@Status

            ";

            var parameters = new
            {
                ModuleTypeName,
                ApproverId,
                Status = MiscEnumEntity.Pending
            };
            var result = await _dbConnection.QueryAsync(dataQuery, parameters);
            return result.ToList();
        }

        public async Task<List<dynamic>> GetAllApprovalRequestByWorkflowType(string ModuleTypeName)
        {
            const string dataQuery = @" WITH RankedApprovals AS (
                SELECT 
                    AR.ModuleTransactionId,
                    AR.Id AS ApprovalRequestId,
                    Status.Code AS CurrentStatus,
                    WorkFlow.ModuleTypeName,
                    ApprovalStep.Code AS ApprovalStep,
                    ASD.StepOrder,
                    ROW_NUMBER() OVER (PARTITION BY AR.ModuleTransactionId ORDER BY ASD.StepOrder DESC) AS rn
                FROM [AppData].[ApprovalRequest] AR
                INNER JOIN [AppData].[ApprovalStepDetail] ASD ON ASD.Id = AR.ApprovalStepDetailId
                INNER JOIN [AppData].[MiscMaster] Status ON Status.Id = AR.StatusId
                INNER JOIN [AppData].[WorkflowType] WorkFlow ON WorkFlow.Id = AR.WorkflowTypeId
                INNER JOIN [AppData].[MiscMaster] ApprovalStep ON ApprovalStep.Id = ASD.ApprovalStepId
                WHERE WorkFlow.ModuleTypeName = @ModuleTypeName
            )
            SELECT 
                ModuleTransactionId,
                ApprovalRequestId,
                CurrentStatus,
                ModuleTypeName,
                ApprovalStep
            FROM RankedApprovals
            WHERE rn = 1;

            ";

            var parameters = new
            {
                ModuleTypeName
            };
            var result = await _dbConnection.QueryAsync(dataQuery, parameters);
            return result.ToList();
        }

        public async Task<List<int>> GetApprovalStepDetailByIdAsync(string WorkFlowType, int ModuleTransactionId, int UnitId, int DepartmentId)
        {
            const string query = @"
                SELECT ASD.Id
            FROM [AppData].[ApprovalStepDetail] ASD
            INNER JOIN [AppData].[ApprovalStepUnitMapping] ASM 
                ON ASM.ApprovalStepDetailId = ASD.Id
            INNER JOIN [AppData].[ApprovalStepDepartmentMapping] ApprovalDept 
                ON ApprovalDept.ApprovalStepDetailId = ASD.Id
            LEFT JOIN [AppData].[ApprovalRequest] AR 
                ON AR.ApprovalStepDetailId = ASD.Id 
                AND AR.WorkflowType = @WorkFlowType 
                AND AR.ModuleTransactionId = @ModuleTransactionId
            WHERE ASD.IsDeleted = 0 
              AND ASD.IsActive = 1 
              AND AR.Id IS NULL AND ASM.UnitId = @UnitId AND ApprovalDept.DepartmentId = @DepartmentId
            ORDER BY ASD.StepOrder ASC;";

            var WorkflowType = await _dbConnection.QueryAsync<int>(query, new { WorkFlowType, ModuleTransactionId, UnitId, DepartmentId });
            return WorkflowType.ToList();
        }
        public async Task<List<int>> StartApprovalProcessAsync(List<int> Id, Dictionary<string, object> requestData)
        {
            var resultIds = new List<int>();
            var steps = await _dbConnection.QueryAsync<dynamic>(
                @"SELECT s.Id, s.StepOrder, s.TargetTypeId, r.ConditionKey, r.Operator, r.Value
                  FROM [AppData].[ApprovalStepDetail] s
                  LEFT JOIN [AppData].[RuleSkipApproverMapping] m ON s.Id = m.ApprovalDetailId
                  LEFT JOIN [AppData].[ApprovalRule] r ON m.RuleId = r.Id
                  WHERE s.Id IN @Id
                  ORDER BY s.StepOrder",
                new { Id });

            var groupedSteps = steps.GroupBy(x => x.Id);

            foreach (var stepGroup in groupedSteps)
            {
                var step = stepGroup.First();
                bool allRulesPass = true;

                foreach (var rule in stepGroup)
                {
                    if (!string.IsNullOrEmpty(rule.ConditionKey))
                    {
                        var actualValue = requestData.ContainsKey(rule.ConditionKey)
                                            ? requestData[rule.ConditionKey]?.ToString()
                                            : null;

                        if (!EvaluateCondition(actualValue, rule.Operator, rule.Value))
                        {
                            allRulesPass = false;
                            break;
                        }
                    }
                }

                // if (!allRulesPass)
                // {
                // await MarkTransactionAsync(requestId, step.UserId, step.StepOrder, "Skipped");
                // continue;
                // }

                // ✅ Send approval request
                // await InsertTransactionAsync(requestId, step, "Pending");
                // var approved = await _approvalService.WaitForApproval(step.UserId, requestData);

                // if (!approved)
                // {
                //     await MarkTransactionAsync(requestId, step.UserId, step.StepOrder, "Rejected");
                //     break;
                // }

                // await MarkTransactionAsync(requestId, step.UserId, step.StepOrder, "Approved");

                if (allRulesPass)
                {
                    resultIds.Add(stepGroup.Key);
                }
            }
            return resultIds;
        }

        private bool EvaluateCondition(string? actualValue, string? op, string? expectedValue)
        {

            if (string.IsNullOrEmpty(op) || string.IsNullOrEmpty(expectedValue))
                return true;


            if (string.IsNullOrEmpty(actualValue))
                return op == "!=" && !string.IsNullOrEmpty(expectedValue);

            switch (op.Trim())
            {
                case "=":
                case "==":
                    return string.Equals(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase);

                case "!=":
                case "<>":
                    return !string.Equals(actualValue, expectedValue, StringComparison.OrdinalIgnoreCase);

                case ">":
                    return TryParseDecimal(actualValue) > TryParseDecimal(expectedValue);

                case "<":
                    return TryParseDecimal(actualValue) < TryParseDecimal(expectedValue);

                case ">=":
                    return TryParseDecimal(actualValue) >= TryParseDecimal(expectedValue);

                case "<=":
                    return TryParseDecimal(actualValue) <= TryParseDecimal(expectedValue);

                case "Contains":
                    return actualValue.Contains(expectedValue, StringComparison.OrdinalIgnoreCase);

                case "NotContains":
                    return !actualValue.Contains(expectedValue, StringComparison.OrdinalIgnoreCase);

                case "In":
                    return expectedValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Any(v => string.Equals(v.Trim(), actualValue, StringComparison.OrdinalIgnoreCase));

                case "NotIn":
                    return !expectedValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Any(v => string.Equals(v.Trim(), actualValue, StringComparison.OrdinalIgnoreCase));

                default:
                    return true;
            }
        }

        private decimal TryParseDecimal(string value)
        {
            return decimal.TryParse(value, out var result) ? result : 0;
        }

    }
}