using Core.Domain.Entities;
using Core.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using Core.Application.Common;


namespace Core.Application.Departments.Queries.GetDepartments
{
    public class DepartmentVm : BaseEntity ,IMapFrom<Department>

    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string DeptName { get; set; }
        public int CompanyId { get; set; }
        public byte  IsActive { get; set; }
      
    }
}   