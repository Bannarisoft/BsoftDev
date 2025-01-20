using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Domain.Entities;
using Core.Application.PwdComplexityRule.Queries;
using Core.Application.PwdComplexityRule.Commands;
using Core.Application.PwdComplexityRule.Commands.CreatePasswordComplexityRule;
using Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule;
using Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRule;
using Core.Application.PwdComplexityRule.Commands.DeletePasswordComplexityRule;



namespace Core.Application.Common.Mappings
{
    public class PasswordComplexityRuleProfile :Profile
    {
        public PasswordComplexityRuleProfile()
        {
           
            CreateMap<CreatePasswordComplexityRuleCommand, Core.Domain.Entities.PasswordComplexityRule>();           
           CreateMap<UpdatePasswordComplexityRuleCommand, Core.Domain.Entities.PasswordComplexityRule>() ;       
           CreateMap <Core.Domain.Entities.PasswordComplexityRule , PwdRuleDto>();   
          CreateMap<PwdRuleStatusDto, Core.Domain.Entities.PasswordComplexityRule>()  
          .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ));

             CreateMap<Core.Domain.Entities.PasswordComplexityRule, PwdRuleDto>();
            CreateMap<PwdRuleStatusDto, Core.Domain.Entities.PasswordComplexityRule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<Core.Domain.Entities.PasswordComplexityRule, PwdRuleDto>();

           
        }
}
}