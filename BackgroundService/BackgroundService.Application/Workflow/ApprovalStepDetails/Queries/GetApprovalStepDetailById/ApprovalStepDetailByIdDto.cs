using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Workflow.ApprovalStepDetails.Queries.GetApprovalStepDetailById
{
    public class ApprovalStepDetailByIdDto
    {
        public int Id { get; set; }
        public int WorkFlowTypeId { get; set; }
        public int StepOrder { get; set; }
        public int TargetTypeId { get; set; }
        public int ApprovalStepId { get; set; }
        public int ApprovalTypeId { get; set; }
        public decimal SLAHours { get; set; }
        public string OnSLAAction { get; set; }
        public byte IsActive { get; set; }
        public List<ApprovalStepUnitMappingByIdDto> ApprovalStepUnitMappings { get; set; }
        public List<ApprovalStepDepartmentMappingByIdDto> ApprovalStepDepartmentMappings { get; set; }
        public List<RuleSkipApproverMappingByIdDto> RuleSkipApproverMappings { get; set; }
    }
}