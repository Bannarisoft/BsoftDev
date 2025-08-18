using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;

namespace BackgroundService.Domain.Entities.Workflow
{
    public class ApprovalRuleCondition : BaseEntity
    {
        public int RuleId { get; set; }
        public int GroupKey { get; set; }
        public int FieldId { get; set; }
        public required string Operator { get; set; }
        public required string RightType { get; set; }
        public string? RightValue { get; set; }
        public string? Aggregate { get; set; }
        public ApprovalRule Rule { get; set; } = null!;
        public ApprovalDataField Field { get; set; } = null!;
    }
}