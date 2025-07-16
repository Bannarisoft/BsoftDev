using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineGroupUsers.Command.CreateMachineGroupUser
{
    public class CreateMachineGroupUserCommand  :IRequest<int>
    {
        public int MachineGroupId { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; }
    }
}