using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Queries.GetRole
{
    public class GetRoleQuery : IRequest<List<UserRoleDto>>
    {
        
    }
}