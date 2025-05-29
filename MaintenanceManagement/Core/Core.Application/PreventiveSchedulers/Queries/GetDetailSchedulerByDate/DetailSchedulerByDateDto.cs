using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Queries.GetDetailSchedulerByDate
{
    public class DetailSchedulerByDateDto
    {
        public int HeaderId { get; set; }
        public int DetailId { get; set; }
        public string PreventiveSchedulerName { get; set; }
        public int MachineGroupId { get; set; }
        public string GroupName { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}