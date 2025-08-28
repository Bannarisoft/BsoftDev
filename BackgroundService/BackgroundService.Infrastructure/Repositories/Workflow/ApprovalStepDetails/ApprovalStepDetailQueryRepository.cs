using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common.Interfaces.IApprovalStepDetail;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Domain.Entities.Workflow;
using Dapper;
using Microsoft.Extensions.DependencyInjection;

namespace BackgroundService.Infrastructure.Repositories.Workflow.ApprovalStepDetails
{
    public class ApprovalStepDetailQueryRepository : IApprovalStepDetailQuery
    {
        private readonly IDbConnection _dbConnection;
        public ApprovalStepDetailQueryRepository([FromKeyedServices("Notification")] IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<bool> AlreadyExistsAsync(int WorkFlowTypeId,int TargetTypeId,int ApprovalStepId,int ApprovalTypeId, int? id = null)
        {
             var query = @"SELECT COUNT(1) FROM [AppData].[ApprovalStepDetail] WHERE WorkFlowTypeId = @WorkFlowTypeId
             
             AND TargetTypeId = @TargetTypeId AND ApprovalStepId = @ApprovalStepId AND ApprovalTypeId = @ApprovalTypeId AND IsDeleted = 0";
            var parameters = new DynamicParameters(new { WorkFlowTypeId,TargetTypeId, ApprovalStepId,ApprovalTypeId  });

            if (id is not null)
            {
                query += " AND Id != @Id";
                parameters.Add("Id", id);
            }
            var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
            return count > 0;
        }

        public async Task<(List<ApprovalStepDetail>, int)> GetAllApprovalStepDetailAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
              

           const string dataQuery =    @" SELECT 
                ASD.Id, 
                ASD.WorkFlowTypeId,
                ASD.StepOrder,
                ASD.TargetTypeId,
                ASD.ApprovalStepId,
                ASD.ApprovalTypeId,
                ASD.SLAHours,
                ASD.OnSLAAction,
                ASD.IsActive,ASD.CreatedDate,ASD.CreatedBy,ASD.CreatedByName,ASD.ModifiedBy,ASD.ModifiedDate,ASD.ModifiedByName,
                ApprovalStep.Id,ApprovalStep.Code,WorkFlow.Id,WorkFlow.ModuleTypeName,ApprovalType.Id,ApprovalType.Code
            FROM [AppData].[ApprovalStepDetail] ASD
            INNER JOIN [AppData].[MiscMaster] ApprovalStep on ApprovalStep.Id=ASD.ApprovalStepId
            INNER JOIN [AppData].[WorkflowType] WorkFlow on WorkFlow.Id=ASD.WorkFlowTypeId
            INNER JOIN [AppData].[MiscMaster] ApprovalType on ApprovalType.Id=ASD.ApprovalTypeId
            WHERE 
            ASD.IsDeleted = 0
                AND (@Search IS NULL OR ApprovalStep.Code LIKE @Search OR WorkFlow.ModuleTypeName LIKE @Search OR ApprovalType.Code LIKE @Search)
                ORDER BY ASD.Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";
              const string countQuery = @"
              SELECT COUNT(*) 
               FROM [AppData].[ApprovalStepDetail] ASD
            INNER JOIN [AppData].[MiscMaster] ApprovalStep on ApprovalStep.Id=ASD.ApprovalStepId
            INNER JOIN [AppData].[WorkflowType] WorkFlow on WorkFlow.Id=ASD.WorkFlowTypeId
            INNER JOIN [AppData].[MiscMaster] ApprovalType on ApprovalType.Id=ASD.ApprovalTypeId
             WHERE ASD.IsDeleted = 0  AND (@Search IS NULL OR ApprovalStep.Code LIKE @Search OR WorkFlow.ModuleTypeName LIKE @Search OR ApprovalType.Code LIKE @Search);
          ";


            var parameters = new
            {
                Search = string.IsNullOrEmpty(SearchTerm) ? null : $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var ApprovalStep = await _dbConnection.QueryAsync<ApprovalStepDetail, Domain.Entities.Notification.MiscMaster, WorkflowType, Domain.Entities.Notification.MiscMaster, ApprovalStepDetail>(
                dataQuery,
                (detail, approvalStep, workFlow, approvalType) =>
                {
                     detail.ApprovalStep = new Domain.Entities.Notification.MiscMaster
                     {
                         Id = approvalStep.Id,
                         Code = approvalStep.Code
                     };
                     detail.WorkflowType = new WorkflowType
                     {
                         Id = workFlow.Id,
                         ModuleTypeName = workFlow.ModuleTypeName
                     };
                    //  detail.ApprovalType = new Domain.Entities.Notification.MiscMaster
                    //  {
                    //      Id = approvalType.Id,
                    //      Code = approvalType.Code
                    //  };
                     return detail;
                },
                parameters,
                splitOn: "Id,Id,Id,Id"                
                );
            
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);

            return (ApprovalStep.ToList(), totalCount);
        }

        public async Task<ApprovalStepDetail> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT ASD.Id,ASD.WorkFlowTypeId,ASD.StepOrder,ASD.TargetTypeId,ASD.ApprovalStepId,ASD.ApprovalTypeId,
                ASD.SLAHours,ASD.OnSLAAction,ASD.IsActive,ASM.Id,ASM.UnitId,ApprovalDept.Id,ApprovalDept.DepartmentId,RSM.Id,RSM.RuleId
            FROM [AppData].[ApprovalStepDetail] ASD
            INNER JOIN [AppData].[ApprovalStepUnitMapping] ASM 
                ON ASM.ApprovalStepDetailId = ASD.Id
            INNER JOIN [AppData].[ApprovalStepDepartmentMapping] ApprovalDept
                ON ApprovalDept.ApprovalStepDetailId = ASD.Id
            LEFT JOIN [AppData].[RuleSkipApproverMapping] RSM
                ON RSM.ApprovalDetailId = ASD.Id
            WHERE ASD.IsDeleted = 0 
              AND ASD.IsActive = 1 AND ASD.Id = @Id;";

              var ApprovalStepDetailDictionary = new Dictionary<int, ApprovalStepDetail>();

            // var ApprovalStepDetailResponse = await _dbConnection.QueryAsync<ApprovalStepDetail, ApprovalStepUnitMapping, ApprovalStepDepartmentMapping, RuleSkipApproverMapping,ApprovalStepDetail>(
            var ApprovalStepDetailResponse = await _dbConnection.QueryAsync<ApprovalStepDetail, ApprovalStepUnitMapping,ApprovalStepDetail>(
                query,
                (approvalStep, approvalStepUnit) =>
                {
                    if (!ApprovalStepDetailDictionary.TryGetValue(approvalStep.Id, out var existingApprovalStep))
                    {
                        existingApprovalStep = approvalStep;
                        existingApprovalStep.ApprovalStepUnitMappings = new List<ApprovalStepUnitMapping>();
                        // existingApprovalStep.ApprovalStepDepartmentMappings = new List<ApprovalStepDepartmentMapping>();
                        // existingApprovalStep.RuleSkipApproverMappings = new List<RuleSkipApproverMapping>();
                        ApprovalStepDetailDictionary[approvalStep.Id] = existingApprovalStep;
                    }

                    if (!existingApprovalStep.ApprovalStepUnitMappings!
                        .Any(a => a.Id == approvalStepUnit.Id))
                    {
                        existingApprovalStep.ApprovalStepUnitMappings.Add(approvalStepUnit);
                    }

                    //  if (!existingApprovalStep.ApprovalStepDepartmentMappings!
                    //     .Any(a => a.Id == approvalStepDepartment.Id))
                    // {
                    //     existingApprovalStep.ApprovalStepDepartmentMappings.Add(approvalStepDepartment);
                    // }
                    // if (!existingApprovalStep.RuleSkipApproverMappings!
                    //     .Any(a => a.Id == ruleSkipApprover.Id))
                    // {
                    //     existingApprovalStep.RuleSkipApproverMappings.Add(ruleSkipApprover);
                    // }

                    return existingApprovalStep;
                },
                new { id },
                splitOn: "Id,Id,Id"
                );

            return ApprovalStepDetailResponse.FirstOrDefault()!;
         
        }

        public async Task<bool> NotFoundAsync(int id)
        {
             var query = "SELECT COUNT(1) FROM [AppData].[ApprovalStepDetail]  WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
        }
    }
}