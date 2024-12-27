using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Companies.Commands.CreateCompany;
using Core.Application.Companies.Queries.GetCompanies;
using Core.Domain.Entities;

namespace Core.Application.Common.Mappings
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            // CreateMap<CreateCompanyCommand, Company>()
            //      .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.CompanyAddresses))
            //      .ForMember(dest => dest.CompanyContact, opt => opt.MapFrom(src => src.CompanyContacts));

                CreateMap<CompanyDTO, Company>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.LegalName, opt => opt.MapFrom(src => src.LegalName))
                .ForMember(dest => dest.GstNumber, opt => opt.MapFrom(src => src.GstNumber))
                .ForMember(dest => dest.TIN, opt => opt.MapFrom(src => src.TIN))
                .ForMember(dest => dest.TAN, opt => opt.MapFrom(src => src.TAN))
                .ForMember(dest => dest.CSTNo, opt => opt.MapFrom(src => src.CSTNo))
                .ForMember(dest => dest.YearOfEstablishment, opt => opt.MapFrom(src => src.YearOfEstablishment))
                .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website))
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo))
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ReverseMap();
                 
        CreateMap<CompanyAddressDTO, CompanyAddress>()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
            .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.AddressLine1))
            .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.AddressLine2))
            .ForMember(dest => dest.PinCode, opt => opt.MapFrom(src => src.PinCode))
            .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
            .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateId))
            .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.AlternatePhone, opt => opt.MapFrom(src => src.AlternatePhone))
            .ReverseMap();
        CreateMap<CompanyContactDTO, CompanyContact>()
        .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.Designation))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ReverseMap();
        
        // // Add mappings for lists
        // CreateMap<List<CompanyAddressDTO>, List<CompanyAddress>>();
        // CreateMap<List<CompanyContactDTO>, List<CompanyContact>>();
                 
        }
             
    }
}