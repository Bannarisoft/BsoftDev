using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.Departments.Queries.GetDepartmentAutoComplete
{
    public class DepartmentAutoCompleteVm  :IMapFrom<Department>
    {
     public int DeptId { get; set; }
    public string DeptName { get; set; }
    }
}