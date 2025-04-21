

using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder.Schedule
{
    public class UpdateWOScheduleCommand : IRequest<ApiResponseDTO<bool>>     
    {
        public WorkOrderScheduleUpdateDto? WOSchedule { get; set; }    
    }
}