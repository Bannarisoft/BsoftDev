using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using BSOFT.Application.Common;


namespace BSOFT.Application.Departments.Queries.GetDepartments
{
    public class DepartmentVm : BaseEntityVm ,IMapFrom<Department>

    {
        public int DeptId { get; set; }
        public string ShortName { get; set; }
        public string DeptName { get; set; }
        public int CoId { get; set; }
        public byte  IsActive { get; set; }
      
    }
}   