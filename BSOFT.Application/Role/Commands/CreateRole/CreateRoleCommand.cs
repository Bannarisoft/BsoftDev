using BSOFT.Application.Role.Queries.GetRole;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Role.Commands.CreateRole
{
    public class CreateRoleCommand  : IRequest<RoleVm>
    {
        
 
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CoId { get; set; }
        public byte  IsActive { get; set; }
        public int CreatedBy  { get; set; }
        public DateTime CreatedAt  { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedIP { get; set; }         
        public int ModifiedBy  { get; set; }
        public DateTime ModifiedAt  { get; set; }
        public string ModifiedByName { get; set; }
        public string ModifiedIP { get; set; } 
    }
}