using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ActivityMaster.Command.CreateActivityMaster;
using Core.Application.ActivityMaster.Command.UpdateActivityMster;
using Core.Application.ActivityMaster.Queries.GetAllActivityMaster;
using Core.Application.MachineGroup.Queries.GetMachineGroupAutoComplete;
using Core.Application.MachineGroup.Queries.GetMachineGroupById;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;


namespace Core.Application.Common.Mappings
{
    public class ActivityMasterProfile :Profile
    {
        public ActivityMasterProfile()
        {
            //CreateMap<Core.Domain.Entities.ActivityMaster, GetAllActivityMasterDto>();
           
            CreateMap<Core.Domain.Entities.ActivityMaster, GetAllActivityMasterDto>(); 
             CreateMap<ActivityMachineGroup, GetAllMachineGroupDto>();
          //  CreateMap<Core.Domain.Entities.ActivityMaster, GetAllMachineGroupDto>();


            CreateMap<Core.Domain.Entities.ActivityMaster, GetActivityMasterByIdDto>();

            CreateMap<Core.Domain.Entities.ActivityMaster, GetActivityMasterAutoCompleteDto>();

            CreateMap<CreateActivityMasterDto, Core.Domain.Entities.ActivityMaster>()
            .ForMember(dest => dest.ActivityMachineGroups, opt => opt.MapFrom(src => src.ActivityMachineGroup)) // Maps List<ActivityMachineGroupDto> to List<ActivityMachineGroup>
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
            
            CreateMap<ActivityMachineGroupDto, ActivityMachineGroup>();


            CreateMap<UpdateActivityMasterDto, Core.Domain.Entities.ActivityMaster>()
           .ForMember(dest => dest.ActivityMachineGroups, opt => opt.MapFrom(src => src.UpdateActivityMachineGroup));// Maps List<ActivityMachineGroupDto> to List<ActivityMachineGroup>          
          //   .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));
            
            CreateMap<UpdateActivityMachineGroupDto, ActivityMachineGroup>();

            // CreateMap<UpdateActivityMasterDto, Core.Domain.Entities.ActivityMaster>()
            //     .ForMember(dest => dest.ActivityMachineGroups, opt => opt.MapFrom(src => 
            //         src.UpdateActivityMachineGroup.Select(x => new ActivityMachineGroup 
            //         {
            //             ActivityMasterId = src.ActivityId,  // Ensure the correct ActivityMasterId is set
            //             MachineGroupId = x.MachineGroupId
            //         }).ToList()
            //     ))
            //     .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            //     .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            // CreateMap<UpdateActivityMachineGroupDto, ActivityMachineGroup>(); 


           

         



        }


    }
}