using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.CompanySettings.Commands.CreateCompanySettings;
using Core.Application.CompanySettings.Commands.UpdateCompanySettings;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.Common.Mappings
{
    public class CompanySettingsProfile : Profile
    {
        public CompanySettingsProfile()
        {
            CreateMap<CreateCompanySettingsCommand, Core.Domain.Entities.CompanySettings>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active));
            CreateMap<UpdateCompanySettingsCommand, Core.Domain.Entities.CompanySettings>();
        }
    }
}