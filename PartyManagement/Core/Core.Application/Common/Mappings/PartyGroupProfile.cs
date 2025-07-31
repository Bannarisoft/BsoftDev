using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.PartyGroup.Command.CreatePartyGroup;
using Core.Application.PartyGroup.Command.DeletePartyGroup;
using Core.Application.PartyGroup.Command.UpdatePartyGroup;
using Core.Application.PartyGroup.Queries.GetPartyGroup;
using Core.Application.PartyGroup.Queries.GetPartyGroupAutoComplete;
using Core.Application.PartyGroup.Queries.GetPartyGroupById;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class PartyGroupProfile : Profile
    {
        public PartyGroupProfile()
        {
            CreateMap<CreatePartyGroupCommand, Core.Domain.Entities.PartyGroup>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsGroup, opt => opt.MapFrom(src => src.IsGroup == 1 ? true : false))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdatePartyGroupCommand, Core.Domain.Entities.PartyGroup>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? Status.Active : Status.Inactive));

            CreateMap<DeletePartyGroupCommand, Core.Domain.Entities.PartyGroup>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted)); 

            CreateMap<Core.Domain.Entities.PartyGroup, PartyGroupDto>();
            CreateMap<Core.Domain.Entities.PartyGroup, PartyGroupByIdDto>();

            CreateMap<Core.Domain.Entities.PartyGroup, PartyGroupAutoCompleteDto>();
        }
    }
}