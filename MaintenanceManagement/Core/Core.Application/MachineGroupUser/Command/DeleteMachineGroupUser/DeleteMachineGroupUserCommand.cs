using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineGroupUser.Command.DeleteMachineGroupUser
{
    public class DeleteMachineGroupUserCommand  : IRequest<bool>
    {
        public int Id { get; set; }
    }
}