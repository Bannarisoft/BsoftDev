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
        CreateMap<MenuPermissionDto, RoleEntitlement>()
            .ForMember(dest => dest.MenuId, opt => opt.MapFrom(src => src.MenuId))
            .ForMember(dest => dest.ModuleId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted))
            .ForMember(dest => dest.CanView, opt => opt.MapFrom(src => src.CanView))
            .ForMember(dest => dest.CanAdd, opt => opt.MapFrom(src => src.CanAdd))
            .ForMember(dest => dest.CanUpdate, opt => opt.MapFrom(src => src.CanUpdate))
            .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.CanDelete))
            .ForMember(dest => dest.CanExport, opt => opt.MapFrom(src => src.CanExport))
            .ForMember(dest => dest.CanApprove, opt => opt.MapFrom(src => src.CanApprove));

        CreateMap<ModuleMenuPermissionDto, RoleEntitlement>()
            // .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId));
            .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

        CreateMap<RoleEntitlement, RoleEntitlementDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.UserRole.RoleName))
            .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module.ModuleName))
            .ForMember(dest => dest.MenuName, opt => opt.MapFrom(src => src.Menu.MenuName)); 
        
         CreateMap<CreateRoleEntitlementCommand, RoleEntitlement>()
        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
        .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
            
        CreateMap<UpdateRoleEntitlementCommand, RoleEntitlement>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));
    //     CreateMap<UpdateRoleEntitlementCommand, List<RoleEntitlement>>()
    // .ConvertUsing((src, dest, context) =>
    // {
    //     return src.ModuleMenus
    //         .SelectMany(moduleMenu => moduleMenu.Menus
    //             .Select(menu => 
    //             {
    //                 var roleEntitlement = dest?.FirstOrDefault(re => re.ModuleId == moduleMenu.ModuleId && re.MenuId == menu.MenuId) 
    //                                      ?? new RoleEntitlement();

    //                 roleEntitlement.ModuleId = moduleMenu.ModuleId;
    //                 roleEntitlement.MenuId = menu.MenuId;
    //                 roleEntitlement.CanView = menu.CanView;
    //                 roleEntitlement.CanAdd = menu.CanAdd;
    //                 roleEntitlement.CanUpdate = menu.CanUpdate;
    //                 roleEntitlement.CanDelete = menu.CanDelete;
    //                 roleEntitlement.CanExport = menu.CanExport;
    //                 roleEntitlement.CanApprove = menu.CanApprove;
    //                 roleEntitlement.IsActive = src.IsActive == 1 ? Status.Active : Status.Inactive;

    //                 return roleEntitlement;
    //             }))
    //         .ToList();
    // });


        CreateMap<DeleteRoleEntitlementCommand, RoleEntitlement>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Map UserRoleId to Id
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));
               
    }
    }
}