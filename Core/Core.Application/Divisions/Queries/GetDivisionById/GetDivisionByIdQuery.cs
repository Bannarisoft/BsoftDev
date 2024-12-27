using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Text;
using Core.Application.Divisions.Queries.GetDivisions;

namespace Core.Application.Divisions.Queries.GetDivisionById
{
    public class GetDivisionByIdQuery : IRequest<DivisionVm>
    {
        public int DivId { get; set; }
    }
}