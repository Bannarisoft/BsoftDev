using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class PreventiveSchedulerHdr : BaseEntity
    {
        public int MachineGroupId { get; set; }
        public MachineGroup MachineGroup { get; set; }
        public int DepartmentId { get; set; }
        public int MaintenanceCategoryId { get; set; }
        public MaintenanceCategory MaintenanceCategory { get; set; }
        public int ScheduleId { get; set; }
        public MiscMaster MiscSchedule { get; set; }
        public int DueTypeId { get; set; }
        public MiscMaster MiscDueType { get; set; }
        public int DuePeriod { get; set; }
        public int FrequencyId { get; set; }
        public MiscMaster MiscFrequency { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public int GraceDays { get; set; }
        public int ReminderWorkOrderDays { get; set; }
        public int ReminderMaterialReqDays { get; set; }
        public byte IsDownTimeRequired { get; set; }
        public decimal DownTimeEstimateHrs { get; set; }
        public ICollection<PreventiveSchedulerDtl>? PreventiveSchedulerDtls { get; set; }
        public ICollection<PreventiveSchedulerActivity>? PreventiveSchedulerActivities { get; set; }
        public ICollection<PreventiveSchedulerItems>? PreventiveSchedulerItems { get; set; }
    }
}