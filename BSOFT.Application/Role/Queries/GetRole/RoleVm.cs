using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using BSOFT.Application.Common;

namespace BSOFT.Application.Role.Queries.GetRole
{
    public class RoleVm : BaseEntityVm,IMapFrom<BSOFT.Domain.Entities.Role>
    {        
 
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CoId { get; set; }
        public byte  IsActive { get; set; }
    }
}