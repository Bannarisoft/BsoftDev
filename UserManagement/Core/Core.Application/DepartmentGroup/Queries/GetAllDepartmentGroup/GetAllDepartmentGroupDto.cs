using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.DepartmentGroup.Queries.GetAllDepartmentGroup
{
    public class GetAllDepartmentGroupDto
    {

         public int Id { get; set; }
        public string? DepartmentGroupCode { get; set; }
        public string? DepartmentGroupName { get; set; }
        public Status  IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }
        public IsDelete  IsDeleted { get; set; }
        
    }
}