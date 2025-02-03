using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Language.Commands.CreateLanguage;
using Core.Application.Language.Queries.GetLanguages;

namespace Core.Application.Common.Mappings
{
    public class LanguageProfile : Profile
    {
        public LanguageProfile()
        {
            CreateMap<CreateLanguageCommand, Core.Domain.Entities.Language>();
            CreateMap<Core.Domain.Entities.Language, LanguageDTO>();
        }
    }
}