using AutoMapper;
using Core.Application.MachineGroupUser.Command.DeleteMachineGroupUser;
using Core.Application.MachineGroupUser.Command.UpdateMachineGroupUser;
using Core.Application.MachineGroupUser.Queries.GetMachineGroupUser;
using Core.Application.MachineGroupUser.Queries.GetMachineGroupUserAutoComplete;
using Core.Application.MachineGroupUsers.Command.CreateMachineGroupUser;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class MachineGroupUserProfile  : Profile
    {
       public MachineGroupUserProfile()
       {
            CreateMap<Core.Domain.Entities.MachineGroupUser, MachineGroupUserDto>();

            CreateMap<CreateMachineGroupUserCommand, Core.Domain.Entities.MachineGroupUser>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
            
             CreateMap<Core.Domain.Entities.MachineGroupUser, MachineGroupUserDto>();

            CreateMap<UpdateMachineGroupUserCommand, Core.Domain.Entities.MachineGroupUser>()
           .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));       

            CreateMap<DeleteMachineGroupUserCommand, Core.Domain.Entities.MachineGroupUser>()           
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));     

            CreateMap<Core.Domain.Entities.MachineGroupUser, MachineGroupUserAutoCompleteDto>();        
       }    
        
    }
}