using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Workflow.WorkflowTypes.Commands.CreateWorkflowType
{
    public class CreateWorkflowTypeCommand : IRequest<int>
    {
        public int ModuleId { get; set; }
        public string ModuleTypeName { get; set; }
    }
}