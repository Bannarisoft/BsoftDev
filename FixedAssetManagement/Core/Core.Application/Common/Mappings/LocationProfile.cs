using AutoMapper;
using Core.Application.Location.Command.CreateLocation;
using Core.Application.Location.Command.DeleteLocation;
using Core.Application.Location.Command.UpdateLocation;
using Core.Application.Location.Queries.GetLocations;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<CreateLocationCommand, Core.Domain.Entities.Location>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateLocationCommand, Core.Domain.Entities.Location>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteLocationCommand,  Core.Domain.Entities.Location>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));
            
            CreateMap< Core.Domain.Entities.Location, LocationDto>();            
            CreateMap< Core.Domain.Entities.Location, LocationAutoCompleteDto>();
        }
        
    }
}