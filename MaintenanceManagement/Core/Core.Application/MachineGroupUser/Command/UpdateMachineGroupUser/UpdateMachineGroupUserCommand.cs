
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineGroupUser.Command.UpdateMachineGroupUser
{
    public class UpdateMachineGroupUserCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int MachineGroupId { get; set; }
        public int DepartmentId { get; set; }
        public int UserId { get; set; }
        public byte IsActive { get; set; }
    }
}