using MediatR;
using Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.RoleEntitlements.Commands.UpdateRoleRntitlement
{
    public class UpdateRoleEntitlementCommand : IRequest<bool>
    {
        public string RoleName { get; set; }
        public List<ModuleMenuPermissionDto> ModuleMenus { get; set; }
    }
}