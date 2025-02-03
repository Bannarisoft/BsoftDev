using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.UserRole.Queries.GetRole
{
    public class GetUserRoleDto
    {
        public int UserRoleId  { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public int CompanyId { get; set; }
        public Status  IsActive { get; set; }
        public IsDelete IsDeleted { get; set; }
    }
}