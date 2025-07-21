using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalStepDetails.Commands.UpdateApprovalStepDetail
{
    public class UpdateApprovalStepDetailCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int WorkFlowTypeId { get; set; }
        public int StepOrder { get; set; }
        public int TargetTypeId { get; set; }
        public int ApprovalStepId { get; set; }
        public int ApprovalTypeId { get; set; }
        public decimal SLAHours { get; set; }
        public string OnSLAAction { get; set; }
    }
}