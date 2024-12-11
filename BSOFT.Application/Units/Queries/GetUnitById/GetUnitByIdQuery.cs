using BSOFT.Application.Units.Queries.GetUnits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Units.Queries.GetUnitById
{
    public class GetUnitByIdQuery : IRequest<UnitVm>
    {
        public int UnitId { get; set; }
    }
}