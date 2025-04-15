using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler
{
    public class CreatePreventiveSchedulerCommand : IRequest<ApiResponseDTO<int>>
    {
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
        public byte IsDownTimeRequired { get; set; }
        public decimal DownTimeEstimateHrs { get; set; }
        public List<PreventiveSchedulerActivityDto> Activity { get; set; }
        public List<PreventiveSchedulerItemsDto> Items { get; set; }
    }
}