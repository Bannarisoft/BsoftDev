using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.RoleEntitlements.Queries.GetRoleEntitlements
{
    public class GetByIdChildMenuDTO
    {
        public int MenuId { get; set; }
        public int ParentId { get; set; }
    }
}