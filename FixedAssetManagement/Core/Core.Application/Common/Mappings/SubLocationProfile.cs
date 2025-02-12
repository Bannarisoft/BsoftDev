using AutoMapper;
using Core.Application.Location.Command.DeleteAubLocation;
using Core.Application.Location.Command.UpdateSubLocation;
using Core.Application.Location.Queries.GetSubLocations;
using Core.Application.SubLocation.Command.CreateSubLocation;
using Core.Application.SubLocation.Queries.GetSubLocations;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class SubLocationProfile : Profile
    {
        public SubLocationProfile()
        {
            CreateMap<CreateSubLocationCommand, Core.Domain.Entities.SubLocation>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateSubLocationCommand, Core.Domain.Entities.SubLocation>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteSubLocationCommand,  Core.Domain.Entities.SubLocation>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));
            
            CreateMap< Core.Domain.Entities.SubLocation, SubLocationDto>();            
            CreateMap< Core.Domain.Entities.SubLocation, SubLocationAutoCompleteDto>();
        }
        
    }
}