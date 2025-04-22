

using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrder.Command.UpdateWorkOrder;
using MediatR;

namespace Core.Application.WorkOrder.Command.CreateWorkOrder.CreateSchedule
{
    public class CreateWOScheduleCommand : IRequest<ApiResponseDTO<bool>>     
    {
        public WorkOrderScheduleUpdateDto? WOSchedule { get; set; }    
    }
}