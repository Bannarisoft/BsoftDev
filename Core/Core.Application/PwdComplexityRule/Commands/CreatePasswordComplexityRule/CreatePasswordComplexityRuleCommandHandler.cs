using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.PwdComplexityRule.Queries;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PwdComplexityRule.Commands.CreatePasswordComplexityRule
{
    public class CreatePasswordComplexityRuleCommandHandler  :IRequestHandler<CreatePasswordComplexityRuleCommand , PwdRuleDto>

    {
          private readonly IPasswordComplexityRepository _passwordComplexityRepository;
           private readonly IMapper _mapper;
           public CreatePasswordComplexityRuleCommandHandler(IPasswordComplexityRepository passwordComplexityRepository ,IMapper mapper )
        {
             _passwordComplexityRepository=passwordComplexityRepository;
               _mapper=mapper;
         
        }

         public async Task<PwdRuleDto> Handle(CreatePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
        {

              var passwordcomplexityruleEntity = _mapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request);
   
            var createpwdcomplexityrule = await _passwordComplexityRepository.CreateAsync(passwordcomplexityruleEntity);
            
            if (createpwdcomplexityrule == null)
            {
                throw new InvalidOperationException("Failed to create user");
            }

            // Map the created User entity to UserDto
            return _mapper.Map<PwdRuleDto>(createpwdcomplexityrule);


        }

        


    }
}