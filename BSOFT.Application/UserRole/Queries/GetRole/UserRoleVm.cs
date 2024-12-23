using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using BSOFT.Application.Common;

namespace BSOFT.Application.UserRole.Queries.GetRole
{
    public class UserRoleVm : BaseEntityVm,IMapFrom<BSOFT.Domain.Entities.UserRole>
    {        
 
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public byte  IsActive { get; set; }
    }
}