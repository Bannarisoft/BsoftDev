
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOderDropdown
{
    public class GetWorkOderDropdownQuery  : IRequest<ApiResponseDTO<List<GetWorkOderDropdownDto>>> 
    {
        
    }
}