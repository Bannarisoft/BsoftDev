using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.UserRole.Commands.DeleteRole
{
    public class DeleteRoleCommand : IRequest<int>
    {
        public int Id { get; set; }        
    }
}