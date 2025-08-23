using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;

namespace BackgroundService.Domain.Entities.Workflow
{
    public class ApprovalDataField : BaseEntity
    {
        public required string FieldKey { get; set; }
        public required string JsonPath { get; set; }
        public required string ValueType { get; set; }
        public required string Scope { get; set; }
        public ICollection<ApprovalRuleCondition> Conditions { get; set; }
    }
}