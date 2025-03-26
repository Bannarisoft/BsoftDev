using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.MachineGroup.Command.CreateMachineGroup;
using Core.Application.MachineGroup.Command.DeleteMachineGroup;
using Core.Application.MachineGroup.Command.UpdateMachineGroup;
using Core.Application.MachineGroup.Quries.GetMachineGroup;
using Core.Application.MachineGroup.Quries.GetMachineGroupAutoComplete;
using Core.Application.MachineGroup.Quries.GetMachineGroupById;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class MachineGroupProfile : Profile
    {

       public MachineGroupProfile()
       {
            CreateMap<Core.Domain.Entities.MachineGroup, MachineGroupDto>();

            CreateMap<CreateMachineGroupCommand, Core.Domain.Entities.MachineGroup>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
            
             CreateMap<Core.Domain.Entities.MachineGroup, GetMachineGroupByIdDto>();

            CreateMap<UpdateMachineGroupCommand, Core.Domain.Entities.MachineGroup>()
           .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));       

            CreateMap<DeleteMachineGroupCommand, Core.Domain.Entities.MachineGroup>()           
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));     

            CreateMap<Core.Domain.Entities.MachineGroup, GetMachineGroupAutoCompleteDto>();        

       }    
        
    }
}