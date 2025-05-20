using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.Departments.Queries.GetDepartments
{
    public class GetDepartmentDto
    {                

         public int Id { get; set; }
    public string? ShortName { get; set; }
    public string? DeptName { get; set; }
    public int CompanyId { get; set; }
    public int DepartmentGroupId { get; set; }
    public string? DepartmentGroupName { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    }
}