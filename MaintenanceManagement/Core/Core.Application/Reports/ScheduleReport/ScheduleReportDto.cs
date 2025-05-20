using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Reports.ScheduleReport
{
    public class ScheduleReportDto
    {
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public string GroupName { get; set; }
        public string MaintenanceCategory { get; set; }
        public string ActivityName { get; set; }
        public string ActivityType { get; set; }
        public string DueDate { get; set; }
        public string LastCompletionDate { get; set; }
    }
}