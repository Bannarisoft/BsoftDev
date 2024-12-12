using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Divisions.Queries.GetDivisions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BSOFT.Application.Divisions.Commands.CreateDivision
{
    public class CreateDivisionCommand : IRequest<DivisionVm>
    {
        public int DivId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public byte IsActive { get; set; }
    }
}