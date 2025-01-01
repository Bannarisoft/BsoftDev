using Core.Application.Departments.Queries.GetDepartments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<DepartmentDto>
    {
    //    public int DeptId { get; set; }
        public string ShortName { get; set; }
        public string DeptName { get; set; }
        public int CompanyId { get; set; }
        public byte  IsActive { get; set; }
         
    }
}