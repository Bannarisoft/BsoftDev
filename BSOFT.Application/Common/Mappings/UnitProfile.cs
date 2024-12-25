using AutoMapper;
using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Application.Units.Commands.CreateUnit;
using BSOFT.Domain.Entities;
using BSOFT.Application.Units.Commands.DeleteUnit;
using BSOFT.Application.Units.Commands.UpdateUnit;
using static BSOFT.Application.Units.Commands.DeleteUnit.DeleteUnitCommand;

namespace BSOFT.Application.Common.Mappings
{
    public class UnitProfile : Profile
    {
        public UnitProfile()
    {
        // Mapping from CreateUnitCommand to Unit
              CreateMap<UnitDto, Unit>()
    .ForMember(dest => dest.Id, opt => opt.Ignore());
    // .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.UnitName))
    // .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.ShortName))
    // .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
    // .ForMember(dest => dest.DivisionId, opt => opt.MapFrom(src => src.DivisionId))
    // .ForMember(dest => dest.UnitHeadName, opt => opt.MapFrom(src => src.UnitHeadName))
    // .ForMember(dest => dest.CINNO, opt => opt.MapFrom(src => src.CINNO))
    // .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));


    
           
            //  CreateMap<UpdateUnitCommand, Unit>()
            // .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.UpdateUnitDto.UnitName))
            // .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.UpdateUnitDto.ShortName))
            // .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.UpdateUnitDto.CompanyId))
            // .ForMember(dest => dest.DivisionId, opt => opt.MapFrom(src => src.UpdateUnitDto.DivisionId))
            // .ForMember(dest => dest.UnitHeadName, opt => opt.MapFrom(src => src.UpdateUnitDto.UnitHeadName))
            // .ForMember(dest => dest.CINNO, opt => opt.MapFrom(src => src.UpdateUnitDto.CINNO))
            // .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.UpdateUnitDto.IsActive));

                // Mapping from UnitAddress to UnitAddressDto
                 CreateMap<UnitAddressDto, UnitAddress>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
                // .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
                // .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateId))
                // .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                // .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.AddressLine1))
                // .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.AddressLine2))
                // .ForMember(dest => dest.PinCode, opt => opt.MapFrom(src => src.PinCode))
                // .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber))
                // .ForMember(dest => dest.AlternateNumber, opt => opt.MapFrom(src => src.AlternateNumber));


            // CreateMap<UnitAddress, UnitAddressDto>()
            // .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
            // .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateId))
            // .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
            // .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.AddressLine1))
            // .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.AddressLine2))
            // .ForMember(dest => dest.PinCode, opt => opt.MapFrom(src => src.PinCode))
            // .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber))
            // .ForMember(dest => dest.AlternateNumber, opt => opt.MapFrom(src => src.AlternateNumber));
             
             // Mapping from UnitContacts to UnitContactsDto

             CreateMap<UnitContactsDto, UnitContacts>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
            // .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            // .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.Designation))
            // .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            // .ForMember(dest => dest.PhoneNo, opt => opt.MapFrom(src => src.PhoneNo))
            // .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks));

            // CreateMap<BSOFT.Application.Units.Commands.UpdateUnit.UpdateUnitContactsDto, UnitContacts>()
            // .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            // .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.Designation))
            // .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            // .ForMember(dest => dest.PhoneNo, opt => opt.MapFrom(src => src.PhoneNo))
            // .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks));

            //  CreateMap<DeleteUnitCommand, Unit>()
            // .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.DeleteUnitDto.IsActive));

              CreateMap<UnitStatusDto, Unit>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));


    } 
    }
}