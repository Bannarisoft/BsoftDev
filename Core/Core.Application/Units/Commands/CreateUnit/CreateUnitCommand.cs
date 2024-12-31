using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Core.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommand : IRequest<int>
    {
         public UnitDto UnitDto { get; set; }
    }



}