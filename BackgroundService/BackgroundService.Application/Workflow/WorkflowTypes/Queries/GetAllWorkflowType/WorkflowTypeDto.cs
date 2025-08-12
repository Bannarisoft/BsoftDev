using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Workflow.WorkflowTypes.Queries.GetAllWorkflowType
{
    public class WorkflowTypeDto
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string ModuleTypeName { get; set; }
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public int ModifiedBy { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string ModifiedByName { get; set; }
    }
}