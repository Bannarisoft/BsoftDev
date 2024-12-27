using Core.Application.UserRole.Queries.GetRole;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Commands.CreateRole
{
    public class CreateRoleCommand  : IRequest<UserRoleVm>
    {
        
 
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public byte  IsActive { get; set; }
       
    }
}