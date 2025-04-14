using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Queries.GetPreventiveScheduler
{
    public class GetPreventiveSchedulerDto
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DuePeriod { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public int GraceDays { get; set; }
        public int ReminderWorkOrderDays { get; set; }
        public int ReminderMaterialReqDays { get; set; }
        public string IsDownTimeRequired { get; set; }
        public string DownTimeEstimateHrs { get; set; }
        public string IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedIP { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedAt { get; set; }
        public string ModifiedByName { get; set; }
        public string ModifiedIP { get; set; }
        public string MachineGroupId { get; set; }
        public string MachineGroup { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ScheduleId { get; set; }
        public string Schedule { get; set; }
        public string DueTypeId { get; set; }
        public string DueType { get; set; }
        public string FrequencyId { get; set; }
        public string Frequency { get; set; }
    }
}