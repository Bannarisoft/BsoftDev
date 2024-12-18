using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommand : IRequest<int>
    {
         public int UnitId { get; set; }
         public byte IsActive { get; set; }

    }
}