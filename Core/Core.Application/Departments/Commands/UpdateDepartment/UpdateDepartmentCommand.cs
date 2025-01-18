using Core.Application.Common;
using Core.Application.Departments.Queries.GetDepartments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommand :  IRequest<Result<int>> 
    {
        public int Id { get; set; }       
        public string ShortName { get; set; }
        public string DeptName { get; set; }
        public int CompanyId { get; set; }
        public byte  IsActive { get; set; }
             
    }
}