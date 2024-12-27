using Core.Domain.Entities;
using Core.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using Core.Application.Common;

namespace Core.Application.UserRole.Queries.GetRole
{
    public class UserRoleVm : BaseEntity,IMapFrom<Core.Domain.Entities.UserRole>
    {        
 
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public byte  IsActive { get; set; }
    }
}