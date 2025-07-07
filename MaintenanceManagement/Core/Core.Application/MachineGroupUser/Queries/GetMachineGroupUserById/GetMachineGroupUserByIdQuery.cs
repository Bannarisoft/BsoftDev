using Core.Application.Common.HttpResponse;
using Core.Application.MachineGroupUser.Queries.GetMachineGroupUser;
using MediatR;

namespace Core.Application.MachineGroupUser.Queries.GetMachineGroupUserById
{
    public class GetMachineGroupUserByIdQuery : IRequest<MachineGroupUserDto>
    {
        public int Id { get; set; }
    }
}