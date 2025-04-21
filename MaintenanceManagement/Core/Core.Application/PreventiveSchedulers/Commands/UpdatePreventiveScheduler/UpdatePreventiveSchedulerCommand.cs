using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler
{
    public class UpdatePreventiveSchedulerCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        public int MachineGroupId { get; set; }
        public int DepartmentId { get; set; }
        public int MaintenanceCategoryId { get; set; }
        public int ScheduleId { get; set; }
        public int FrequencyTypeId { get; set; }
        public int FrequencyInterval { get; set; }
        public int FrequencyUnitId { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public int GraceDays { get; set; }
        public int ReminderWorkOrderDays { get; set; }
        public int ReminderMaterialReqDays { get; set; }
        public byte IsDownTimeRequired { get; set; }
        public decimal DownTimeEstimateHrs { get; set; }
        public byte IsActive { get; set; }
        public List<PreventiveSchedulerActivityUpdateDto>? Activity { get; set; }
        public List<PreventiveSchedulerItemUpdateDto>? Items { get; set; }
    }
}