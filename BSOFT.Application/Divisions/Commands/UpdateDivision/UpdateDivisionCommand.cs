using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BSOFT.Application.Divisions.Commands.UpdateDivision
{
    public class UpdateDivisionCommand : IRequest<int>
    {
         public int DivId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public byte IsActive { get; set; }
    }
}