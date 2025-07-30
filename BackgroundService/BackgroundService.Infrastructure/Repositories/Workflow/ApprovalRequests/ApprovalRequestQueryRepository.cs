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
                const string dataQuery =    @" SELECT 
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

            var ApprovalRequest = await _dbConnection.QueryAsync<ApprovalRequest, ApprovalStepDetail, MiscMaster, WorkflowType, ApprovalRequest>(
                dataQuery,
                (approvalReq,detail, status, workFlow) =>
                {
                     approvalReq.ApprovalStepDetail = new ApprovalStepDetail
                     {
                         TargetTypeId = detail.TargetTypeId
                     };
                     approvalReq.Status = new MiscMaster
                     {
                         Code = status.Code
                     };
                     approvalReq.WorkflowType = new WorkflowType
                     {
                         ModuleTypeName = workFlow.ModuleTypeName
                     };
                     return approvalReq;
                },
                parameters,
                splitOn: "TargetTypeId,Code,ModuleTypeName"                
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

        public async Task<int?> GetApprovalStepDetailByIdAsync(int WorkFlowTypeId, int ModuleTransactionId,int UnitId,int DepartmentId)
        {
            const string query = @"
                SELECT TOP 1 ASD.Id
            FROM [AppData].[ApprovalStepDetail] ASD
            INNER JOIN [AppData].[ApprovalStepUnitMapping] ASM 
                ON ASM.ApprovalStepDetailId = ASD.Id
            INNER JOIN [AppData].[ApprovalStepDepartmentMapping] ApprovalDept 
                ON ApprovalDept.ApprovalStepDetailId = ASD.Id
            LEFT JOIN [AppData].[ApprovalRequest] AR 
                ON AR.ApprovalStepDetailId = ASD.Id 
                AND AR.WorkflowTypeId = @WorkFlowTypeId 
                AND AR.ModuleTransactionId = @ModuleTransactionId
            WHERE ASD.IsDeleted = 0 
              AND ASD.IsActive = 1 
              AND AR.Id IS NULL AND ASM.UnitId = @UnitId AND ApprovalDept.DepartmentId = @DepartmentId
            ORDER BY ASD.StepOrder ASC;";
                
            var WorkflowType = await _dbConnection.QueryAsync<int>(query, new { WorkFlowTypeId, ModuleTransactionId,UnitId, DepartmentId });
            return WorkflowType.FirstOrDefault();
        }
    }
}