using AutoMapper;
using BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Common.Mappings
{
    public class RoleEntitlementMappingProfile : Profile
    {
    public RoleEntitlementMappingProfile()
    {
        CreateMap<MenuPermissionDto, RoleEntitlement>();
    }
        
    }
}