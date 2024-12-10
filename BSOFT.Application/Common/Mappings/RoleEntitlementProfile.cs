using AutoMapper;
using BSOFT.Application.RoleEntitlements.Commands;
using BSOFT.Application.RoleEntitlements.Queries.GetRoles;
using BSOFT.Domain.Entities;

namespace BSOFT.Application.Common.Mappings
{
    public class RoleEntitlementProfile : Profile
    {
        public RoleEntitlementProfile()
        {
            CreateMap<CreateRoleEntitlementDto, RoleEntitlement>();
            CreateMap<MenuPermissionDto, MenuPermission>();
            CreateMap<RoleEntitlement, RoleEntitlementDto>();
            CreateMap<MenuPermission, MenuPermissionDto>();

            
        }
        
    }
}