using BSOFT.Application.RoleEntitlements.Queries.GetRoles;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement
{
    public class CreateRoleEntitlementCommand : IRequest<int>
    {
        public CreateRoleEntitlementDto RoleEntitlementDto { get; set; }
    }
}