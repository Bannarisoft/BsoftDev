using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Queries.GetMachineDetailById
{
    public class MachineDetailBySchedulerIdDto
    {
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public DateOnly WorkOrderCreationStartDate { get; set; }
        public DateOnly LastMaintenanceActivityDate { get; set; }
    }
}