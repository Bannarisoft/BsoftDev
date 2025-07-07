using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MachineGroup.Queries.GetMachineGroup;
using MediatR;

namespace Core.Application.MachineGroup.Command.DeleteMachineGroup
{
    public class DeleteMachineGroupCommand : IRequest<bool>
    {
         public int Id { get; set; }
    }
}