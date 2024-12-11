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
         public int DeptId { get; set; }
        public string ShortName { get; set; }
        public string DeptName { get; set; }
        public int CoId { get; set; }
        public byte  IsActive { get; set; }
        public int CreatedBy  { get; set; }
        public DateTime CreatedAt  { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedIP { get; set; }         
        public int ModifiedBy  { get; set; }
        public DateTime ModifiedAt  { get; set; }
        public string ModifiedByName { get; set; }
        public string ModifiedIP { get; set; }   
    }
}