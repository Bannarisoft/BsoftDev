using AutoMapper;
using Core.Application.UserRole.Queries.GetRole;
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
            // Domain -> DTO Mapping
            CreateMap<Core.Domain.Entities.UserRole, UserRoleDto>();


            // Command -> Domain Mapping
            CreateMap<CreateRoleCommand, Core.Domain.Entities.UserRole>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ensures Id is not set during creation
            
            CreateMap<UpdateRoleCommand, Core.Domain.Entities.UserRole>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Explicitly map Id in update scenarios
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

               CreateMap<DeleteRoleCommand, Core.Domain.Entities.UserRole>();

               CreateMap<Core.Domain.Entities.UserRole, UserRoleDto>();
            CreateMap<UserRoleStatusDto, Core.Domain.Entities.UserRole>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
         
        }

    }
}
