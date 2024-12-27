using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement
{
    public class CreateRoleEntitlementCommand : IRequest<int>
    {
    public required string RoleName { get; set; }
    public List<ModuleMenuPermissionDto>? ModuleMenus { get; set; }
    }

}