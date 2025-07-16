using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ActivityCheckListMaster.Command.CreateActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Command.DeleteActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Command.UpdateActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMaster;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class ActivityCheckListMasterProfile : Profile
    {
        public ActivityCheckListMasterProfile()
        {
            CreateMap<Core.Domain.Entities.ActivityCheckListMaster, GetAllActivityCheckListMasterDto>();           

            CreateMap<CreateActivityCheckListMasterCommand, Core.Domain.Entities.ActivityCheckListMaster>();

            CreateMap<UpdateActivityCheckListMasterCommand, Core.Domain.Entities.ActivityCheckListMaster>()
           .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? Status.Active : Status.Inactive));            
        
            CreateMap<DeleteActivityCheckListMasterCommand, Core.Domain.Entities.ActivityCheckListMaster>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));                      


        }
        
    }
}