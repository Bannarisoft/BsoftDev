using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkOrder.Command.DeleteFileWorkOrder
{
    public class DeleteFileWorkOrderCommand : IRequest<ApiResponseDTO<bool>>
    {
        public string? Image { get; set; }        
    }
}