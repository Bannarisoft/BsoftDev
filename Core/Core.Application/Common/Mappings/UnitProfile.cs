using AutoMapper;
using Core.Application.Units.Queries.GetUnits;
using Core.Application.Units.Commands.CreateUnit;
using Core.Domain.Entities;
using Core.Application.Units.Commands.DeleteUnit;
using Core.Application.Units.Commands.UpdateUnit;
using static Core.Application.Units.Commands.DeleteUnit.DeleteUnitCommand;

namespace Core.Application.Common.Mappings
{
    public class UnitProfile : Profile
    {
        public UnitProfile()
    {
        // Mapping from CreateUnitCommand to Unit
              CreateMap<UnitDto, Unit>()
    .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Mapping from UnitAddress to UnitAddressDto
              CreateMap<UnitAddressDto, UnitAddress>()
    .ForMember(dest => dest.Id, opt => opt.Ignore());
             
        // Mapping from UnitContacts to UnitContactsDto

              CreateMap<UnitContactsDto, UnitContacts>()
    .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Mapping from UnitStatus to UnitStatusDto

              CreateMap<UnitStatusDto, Unit>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));


    } 
    }
}