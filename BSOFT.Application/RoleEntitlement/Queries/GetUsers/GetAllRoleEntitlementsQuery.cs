using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlement.Queries.GetUsers
{
    public class GetAllRoleEntitlementsQuery : IRequest<List<RoleEntitlementDto>>
    {
        
    }
}