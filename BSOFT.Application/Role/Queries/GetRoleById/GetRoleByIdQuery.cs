
using BSOFT.Application.Role.Queries.GetRole;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Role.Queries.GetRoleById
{
    public class GetRoleByIdQuery :IRequest<RoleVm>
    {
      public int RoleId { get; set; }
    }
}