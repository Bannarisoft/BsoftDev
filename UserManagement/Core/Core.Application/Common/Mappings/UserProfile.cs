using AutoMapper;
using Core.Application.Users.Queries.GetUsers;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Commands.UpdateUser;
using Core.Application.Users.Commands.DeleteUser;

using Core.Domain.Entities;
using static Core.Domain.Enums.Common.Enums;
using Core.Application.Users.Queries.GetUserAutoComplete;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserCommand, User>()
        .ForMember(dest => dest.UserCompanies, opt => opt.MapFrom(src => src.UserCompanies))
        .ForMember(dest => dest.UserRoleAllocations, opt => opt.MapFrom(src => src.userRoleAllocations))
        .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
        .ForMember(dest => dest.IsFirstTimeUser, opt => opt.MapFrom(src => FirstTimeUserStatus.Yes))
        .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

        CreateMap<UserCompanyDTO, UserCompany>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<UserRoleAllocationDTO, UserRoleAllocation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(src => src.UserRoleId))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<UserUnitDTO, UserUnit>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            // .ForMember(dest => dest.UserCompanies, opt => opt.MapFrom(src => src.UserCompanies))
            // .ForMember(dest => dest.userRoleAllocations, opt => opt.MapFrom(src => src.UserRoleAllocations))
            // .ForMember(dest => dest.UserUnits, opt => opt.MapFrom(src => src.UserUnits));

        CreateMap<UpdateUserCommand, User>()
        .ForMember(dest => dest.UserCompanies, opt => opt.MapFrom(src => src.UserCompanies))
        .ForMember(dest => dest.UserRoleAllocations, opt => opt.MapFrom(src => src.userRoleAllocations))
        .ForMember(dest => dest.UserUnits, opt => opt.MapFrom(src => src.userUnits))
        .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
        .ForMember(dest => dest.IsFirstTimeUser, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

        CreateMap<DeleteUserCommand, User>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Map UserRoleId to Id
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));

             CreateMap<UserCompany, UserCompanyDTO>()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId));

        CreateMap<UserRoleAllocation, UserRoleAllocationDTO>()
            .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(src => src.UserRoleId));

        CreateMap<UserUnit, UserUnitDTO>()
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId));

            CreateMap<User, UserByIdDTO>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) 
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserCompanies, opt => opt.MapFrom(src => src.UserCompanies))
            .ForMember(dest => dest.userRoleAllocations, opt => opt.MapFrom(src => src.UserRoleAllocations))
            .ForMember(dest => dest.UserUnits, opt => opt.MapFrom(src => src.UserUnits));
            CreateMap<User, UserAutoCompleteDto>();

            CreateMap<PasswordLogDTO, PasswordLog>();
            
    }
}
