using AutoMapper;
using Core.Application.UserRole.Queries.GetRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Application.UserRole.Commands.CreateRole;
using Core.Application.UserRole.Commands.UpdateRole;
using Core.Application.UserRole.Commands.DeleteRole;

namespace Core.Application.Common.Mappings
{
    public class UserRoleProfile : Profile
    {

        public UserRoleProfile()
        {   
            CreateMap< Core.Domain.Entities.UserRole, UserRoleDto>();
            CreateMap<RoleStatusDto,  Core.Domain.Entities.UserRole>()
             .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

             CreateMap<CreateRoleCommand, Core.Domain.Entities.UserRole>();
              CreateMap<UpdateRoleCommand, Core.Domain.Entities.UserRole>() ;
               CreateMap<DeleteRoleCommand, Core.Domain.Entities.UserRole>() ;
        }
       
    }
}