using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineGroupUser.Queries.GetMachineGroupUser
{
    public class GetMachineGroupUserQuery  : IRequest<ApiResponseDTO<List<MachineGroupUserDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SearchTerm { get; set; }
    }
}