using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.PwdComplexityRule.Queries;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using Core.Application.Common;
using Core.Domain.Events;

namespace Core.Application.PwdComplexityRule.Commands.CreatePasswordComplexityRule
{
    public class CreatePasswordComplexityRuleCommandHandler  :IRequestHandler<CreatePasswordComplexityRuleCommand , Result<PwdRuleDto>>

    {
          private readonly IPasswordComplexityRuleCommandRepository _passwordComplexityRepository;
           private readonly IMapper _mapper;
           private readonly IMediator _mediator; 
           public CreatePasswordComplexityRuleCommandHandler(IPasswordComplexityRuleCommandRepository passwordComplexityRepository ,IMapper mapper,IMediator mediator )
        {
             _passwordComplexityRepository=passwordComplexityRepository;
               _mapper=mapper;
               _mediator=mediator;
         
        }

         public async Task<Result<PwdRuleDto>> Handle(CreatePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
        {
         
             var passwordcomplexityruleEntity = _mapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request);

             var result = await _passwordComplexityRepository.CreateAsync(passwordcomplexityruleEntity);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: result.Id.ToString(),
                actionName: result.PwdComplexityRule,
                details: $"Country '{result.PwdComplexityRule}' was created. CountryCode: {result.Id}",
                module:"Country"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var PwdDto = _mapper.Map<PwdRuleDto>(result);
            return Result<PwdRuleDto>.Success(PwdDto);


        }

        


    }
}