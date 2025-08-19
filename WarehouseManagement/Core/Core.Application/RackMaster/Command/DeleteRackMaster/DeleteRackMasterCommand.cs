using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.RackMaster.Command.DeleteRackMaster
{
    public class DeleteRackMasterCommand : IRequest<bool>
    {
        public int  Id { get; set; }
    }
}