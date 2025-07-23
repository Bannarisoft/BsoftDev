using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Domain.Entities.Workflow
{
    public class ApprovalRule : BaseEntity
    {
        public required string ConditionKey { get; set; }
        public required string Operator { get; set; }
        public required string Value { get; set; }
        public string? Action { get; set; }
        public int UnitId { get; set; }
        public int WorkflowTypeId { get; set; }
        public WorkflowType WorkflowType { get; set; }
    }
}