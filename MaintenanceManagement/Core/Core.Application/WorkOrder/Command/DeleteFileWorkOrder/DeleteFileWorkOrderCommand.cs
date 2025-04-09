using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkOrder.Command.DeleteFileWorkOrder
{
    public class DeleteFileWorkOrderCommand : IRequest<ApiResponseDTO<bool>>
    {
        public string? assetPath { get; set; }
        public string? CompanyName { get; set; }  
        public string? UnitName { get; set; } 
    }
}