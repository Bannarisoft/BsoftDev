using AutoMapper;
using Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using Core.Application.RoleEntitlements.Commands.DeleteRoleEntitlement;
using Core.Application.RoleEntitlements.Commands.UpdateRoleRntitlement;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;
using Core.Application.UserRole.Commands.CreateRole;
using Core.Domain.Entities;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.Common.Mappings
{
    public class RoleEntitlementMappingProfile : Profile
    {
    public RoleEntitlementMappingProfile()
    {
        
        CreateMap<ModuleMenuPermissionDto,RoleModule>()
        .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
        .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId))
        .ForMember(dest => dest.RoleMenus, opt => opt.MapFrom(src => src.MenuPermissions));

        CreateMap<MenuPermissionDto, RoleMenu>();

            // CreateMap<RoleDto, RoleEntitlement>()
            // .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(src => src.RoleId));
        
        CreateMap<Core.Domain.Entities.UserRole,RoleDto>();
        CreateMap<RoleModule,GetByIdModuleDTO>()
         .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId));
         CreateMap<Menu,MenuDTO>()
            .ForMember(dest => dest.ChildMenu, opt => opt.MapFrom(src => src.ChildMenus));

            CreateMap<Core.Domain.Entities.UserRole,RoleDto>()
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Core.Domain.Entities.RoleMenu,GetByIdPermissionDTO>();
          
    
               
    }
    }
}