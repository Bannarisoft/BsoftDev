using AutoMapper;
using Core.Application.UOM.Command.CreateUOM;
using Core.Application.UOM.Command.DeleteUOM;
using Core.Application.UOM.Command.UpdateUOM;
using Core.Application.UOM.Queries.GetUOMs;
using Core.Application.UOM.Queries.GetUOMTypeAutoComplete;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class UOMProfile : Profile
    {    
        public UOMProfile()
        {
            CreateMap<CreateUOMCommand, Core.Domain.Entities.UOM>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateUOMCommand, Core.Domain.Entities.UOM>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteUOMCommand,  Core.Domain.Entities.UOM>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));
            CreateMap<Domain.Entities.UOM, UOMDto>();
            CreateMap<Domain.Entities.UOM, UOMAutoCompleteDto>();
            // CreateMap<Domain.Entities.UOM, UOMTypeAutoCompleteDto>();

    //         CreateMap<Domain.Entities.UOM, UOMTypeAutoCompleteDto>()
    // .ForMember(dest => dest.UOMType, opt => opt.MapFrom(src => src.UOMType.Code));


        }
        
    }
}