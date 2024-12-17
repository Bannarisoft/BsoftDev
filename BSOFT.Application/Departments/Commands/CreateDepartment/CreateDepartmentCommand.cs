using BSOFT.Application.Departments.Queries.GetDepartments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<DepartmentVm>
    {
    //    public int DeptId { get; set; }
        public string ShortName { get; set; }
        public string DeptName { get; set; }
        public int CoId { get; set; }
        public byte  IsActive { get; set; }
         
    }
}