using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PreventiveSchedulers.Queries.GetPreventiveSchedulerById
{
    public class PreventiveSchedulerHdrByIdDto
    {
        public int Id { get; set; }
        public int MachineGroupId { get; set; }
        public int DepartmentId { get; set; }
        public int MaintenanceCategoryId { get; set; }
        public int ScheduleId { get; set; }
        public int DueTypeId { get; set; }
        public int DuePeriod { get; set; }
        public int FrequencyId { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public int GraceDays { get; set; }
        public int ReminderWorkOrderDays { get; set; }
        public int ReminderMaterialReqDays { get; set; }
        public int IsDownTimeRequired { get; set; }
        public decimal DownTimeEstimateHrs { get; set; }
        public byte IsActive { get; set; }
        public List<PreventiveSchedulerActivityByIdDto> Activity { get; set; }
        public List<PreventiveSchedulerItemByIdDto> Items { get; set; }
    }
}