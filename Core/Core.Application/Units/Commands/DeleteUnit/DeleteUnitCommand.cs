using Core.Application.Units.Queries.GetUnits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Core.Application.Units.Commands.DeleteUnit.DeleteUnitCommand;

namespace Core.Application.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommand : IRequest<int>
    {
     public int UnitId { get; set; }
     public UnitStatusDto UpdateUnitStatusDto { get; set; }

    }
 
    }
    
