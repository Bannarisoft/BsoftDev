
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkOrder.Command.DeleteFileWorkOrder.Item
{
    public class DeleteFileItemCommand : IRequest<ApiResponseDTO<bool>>
    {
        public string? Image { get; set; }        
    }
}