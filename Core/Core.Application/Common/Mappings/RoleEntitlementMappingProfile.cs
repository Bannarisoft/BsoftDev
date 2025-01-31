using AutoMapper;
using Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using Core.Application.RoleEntitlements.Queries.GetRoleEntitlements;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Mappings
{
    public class RoleEntitlementMappingProfile : Profile
    {
    public RoleEntitlementMappingProfile()
    {
        CreateMap<MenuPermissionDto, RoleEntitlement>()
            .ForMember(dest => dest.MenuId, opt => opt.MapFrom(src => src.MenuId))
            .ForMember(dest => dest.ModuleId, opt => opt.Ignore())
            .ForMember(dest => dest.CanView, opt => opt.MapFrom(src => src.CanView))
            .ForMember(dest => dest.CanAdd, opt => opt.MapFrom(src => src.CanAdd))
            .ForMember(dest => dest.CanUpdate, opt => opt.MapFrom(src => src.CanUpdate))
            .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.CanDelete))
            .ForMember(dest => dest.CanExport, opt => opt.MapFrom(src => src.CanExport))
            .ForMember(dest => dest.CanApprove, opt => opt.MapFrom(src => src.CanApprove));

        CreateMap<ModuleMenuPermissionDto, RoleEntitlement>()
            .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId));

        CreateMap<RoleEntitlement, RoleEntitlementDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.UserRole.RoleName))
            .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module.ModuleName))
            .ForMember(dest => dest.MenuName, opt => opt.MapFrom(src => src.Menu.MenuName)); 
               
    }
    }
}