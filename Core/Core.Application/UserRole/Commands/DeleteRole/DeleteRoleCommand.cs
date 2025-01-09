using Core.Application.UserRole.Queries.GetRole;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Commands.DeleteRole
{
    public class DeleteRoleCommand : IRequest<int>
    {
        public int UserRoleId { get; set; }  

        public byte IsActive { get; set; } 
    }
}