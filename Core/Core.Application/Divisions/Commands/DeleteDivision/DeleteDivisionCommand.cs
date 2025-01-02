using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Reflection;
using System.Text;

namespace Core.Application.Divisions.Commands.DeleteDivision
{
    public class DeleteDivisionCommand : IRequest<int>
    {
        public int Id { get; set; }
        public byte IsActive { get; set; }
    }
}