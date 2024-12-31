using Core.Application.Units.Queries.GetUnits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Units.Queries.GetUnitById
{
    public class GetUnitByIdQuery : IRequest<List<UnitDto>>
    { 
        public int Id { get; set; }
    }
    
}