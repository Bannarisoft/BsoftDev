using Core.Domain.Entities;
using Core.Application.Common.Mappings;

namespace Core.Application.Departments.Queries.GetDepartmentAutoComplete
{
    public class DepartmentAutoCompleteVm  :IMapFrom<Department>
    {
     public int Id { get; set; }
    public string DeptName { get; set; }
    }
}