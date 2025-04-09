using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class GetWorkOrderQuery  : IRequest<ApiResponseDTO<List<WorkOrderDto>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
    }
}