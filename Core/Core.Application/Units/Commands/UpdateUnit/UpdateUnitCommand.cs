using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommand : IRequest<int>
    {    
    public int UnitId  { get; set; }
    public UnitDto UpdateUnitDto { get; set; }  
    }
}