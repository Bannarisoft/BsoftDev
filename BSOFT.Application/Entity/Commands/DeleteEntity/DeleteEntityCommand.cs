using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Entity.Commands.DeleteEntity
{
    public class DeleteEntityCommand : IRequest<int>
    {
        public int EntityId { get; set; }
    }
}