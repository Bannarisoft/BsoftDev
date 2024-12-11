
using BSOFT.Application.Departments.Queries.GetDepartments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQuery :IRequest<DepartmentVm>
    {
        
        public int DepartmentId { get; set; }
        
    }
}