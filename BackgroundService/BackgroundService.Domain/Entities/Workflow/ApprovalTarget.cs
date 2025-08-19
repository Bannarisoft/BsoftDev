using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;
using BackgroundService.Domain.Entities.Notification;

namespace BackgroundService.Domain.Entities.Workflow
{
    public class ApprovalTarget : BaseEntity
    {
        public int ApprovalStepDetailId { get; set; }
        public required string Binding { get; set; }
        public required string Value { get; set; }
        public ApprovalStepDetail ApprovalStepDetail { get; set; }
    }
}