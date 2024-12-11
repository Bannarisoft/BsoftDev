using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommand :IRequest<int>
    {
        public int DeptId { get; set; }
    }
}