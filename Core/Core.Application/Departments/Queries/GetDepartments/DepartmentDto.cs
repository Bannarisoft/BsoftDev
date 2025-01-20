using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Application.Common.Mappings;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Departments.Queries.GetDepartments
{
    public class DepartmentDto  : IMapFrom<ApiResponseDTO<Department>>
    {
         public int Id { get; set; }
        public string ShortName { get; set; }
        public string DeptName { get; set; }
        public int CompanyId { get; set; }
        public byte  IsActive { get; set; }
      
    }
}