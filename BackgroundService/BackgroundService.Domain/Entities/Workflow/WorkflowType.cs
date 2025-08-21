using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;

namespace BackgroundService.Domain.Entities.Workflow
{
    public class WorkflowType : BaseEntity
    {
        public int ModuleId { get; set; }
        public required string ModuleTypeName { get; set; }
        public ICollection<ApprovalStepDetail> ApprovalStepDetails { get; set; }
        public ICollection<ApprovalRule> ApprovalRules { get; set; }
        
    }
}