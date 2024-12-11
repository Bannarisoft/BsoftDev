using AutoMapper;
using BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using BSOFT.Application.RoleEntitlements.Queries.GetRoles;
using BSOFT.Domain.Entities;

public class RoleEntitlementProfile : Profile
    {
        public RoleEntitlementProfile()
        {
            CreateMap<CreateRoleEntitlementVm, RoleEntitlement>();
            CreateMap<MenuPermissionVm, MenuPermission>();
            CreateMap<RoleEntitlement, RoleEntitlementVm>();
            CreateMap<MenuPermission, MenuPermissionVm>();

            
        }
        
    }
