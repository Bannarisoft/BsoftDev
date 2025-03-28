using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ActivityMaster.Command.CreateActivityMaster;
using Core.Application.ActivityMaster.Queries.GetAllActivityMaster;
using Core.Application.MachineGroup.Queries.GetMachineGroupAutoComplete;
using Core.Application.MachineGroup.Queries.GetMachineGroupById;
using static Core.Domain.Common.BaseEntity;


namespace Core.Application.Common.Mappings
{
    public class ActivityMasterProfile :Profile
    {
        public ActivityMasterProfile()
        {
            CreateMap<Core.Domain.Entities.ActivityMaster, GetAllActivityMasterDto>();

            CreateMap<Core.Domain.Entities.ActivityMaster, GetActivityMasterByIdDto>();

            CreateMap<Core.Domain.Entities.ActivityMaster, GetActivityMasterAutoCompleteDto>();

             CreateMap<CreateActivityMasterCommand, Core.Domain.Entities.ActivityMaster>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

         



        }


    }
}