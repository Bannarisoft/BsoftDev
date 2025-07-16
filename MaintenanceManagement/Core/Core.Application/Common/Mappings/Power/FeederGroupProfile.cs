using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Power.FeederGroup.Command.CreateFeederGroup;
using Core.Application.Power.FeederGroup.Command.DeleteFeederGroup;
using Core.Application.Power.FeederGroup.Command.UpdateFeederGroup;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroup;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroupAutoComplete;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroupById;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings.Power
{
    public class FeederGroupProfile : Profile
    {


        public FeederGroupProfile()
        {
            CreateMap<Core.Domain.Entities.Power.FeederGroup, FeederGroupDto>();
            CreateMap<Core.Domain.Entities.Power.FeederGroup, GetFeederGroupByIdDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == Status.Active));

            CreateMap<Core.Domain.Entities.Power.FeederGroup, GetFeederGroupAutoCompleteDto>();

            CreateMap<CreateFeederGroupCommand, Core.Domain.Entities.Power.FeederGroup>();
            CreateMap<UpdateFeederGroupCommand, Core.Domain.Entities.Power.FeederGroup>()
              .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => (Status)src.IsActive));

            CreateMap<DeleteFeederGroupCommand, Core.Domain.Entities.Power.FeederGroup>()
             .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));
             
             
        }
        
    }
}