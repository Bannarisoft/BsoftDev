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
using Microsoft.Extensions.Logging;
using Core.Application.Common.HttpResponse;

namespace Core.Application.PwdComplexityRule.Commands.CreatePasswordComplexityRule
{
    public class CreatePasswordComplexityRuleCommandHandler  :IRequestHandler<CreatePasswordComplexityRuleCommand , ApiResponseDTO<PwdRuleDto>>

    {
          private readonly IPasswordComplexityRuleCommandRepository _passwordComplexityRepository;
           private readonly IMapper _mapper;
           private readonly IMediator _mediator; 
           private readonly ILogger<CreatePasswordComplexityRuleCommandHandler> _logger;
           public CreatePasswordComplexityRuleCommandHandler(IPasswordComplexityRuleCommandRepository passwordComplexityRepository ,IMapper mapper,IMediator mediator,ILogger<CreatePasswordComplexityRuleCommandHandler> logger)
        {
             _passwordComplexityRepository=passwordComplexityRepository;
               _mapper=mapper;
               _mediator=mediator;
               _logger=logger;
         
        }
         public async Task<ApiResponseDTO<PwdRuleDto>> Handle(CreatePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
        {         
          _logger.LogInformation($"Handling CreatePasswordComplexityRuleCommand for Password Complexity Rule: {request.PwdComplexityRule}");
     
                var exists = await _passwordComplexityRepository.ExistsByCodeAsync(request.PwdComplexityRule);
                    if (exists)
                    {
                       _logger.LogWarning($"PasswordComplexityRule {request.PwdComplexityRule} already exists" );
                       return new ApiResponseDTO<PwdRuleDto>
                        {
                        IsSuccess = false,
                        Message = "Password Complexity Rule Name already exists."
                        };
                    }
                var passwordComplexityRuleEntity = _mapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request);
                var result = await _passwordComplexityRepository.CreateAsync(passwordComplexityRuleEntity);

                if (result is null)
            {
                _logger.LogWarning($"Failed to create Password Complexity Rule. Password Complexity Rule entity: {passwordComplexityRuleEntity}");
                return new ApiResponseDTO<PwdRuleDto>
                {
                    IsSuccess = false,
                    Message = "Password Complexity Rule not created"
                };
            }

                _logger.LogInformation($"Password Complexity Rule created successfully with ID: { result.Id}");

            
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Create",
                    actionCode: result.Id.ToString(),
                    actionName: result.PwdComplexityRule,
                    details: $"Password Complexity Rule '{result.PwdComplexityRule}' was created. Rule ID: {result.Id}",
                    module: "PasswordComplexityRule"
                );


                  await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation($"AuditLogsDomainEvent published for Department ID: {result.Id}");

        
            var ruleDto = _mapper.Map<PwdRuleDto>(result);

            _logger.LogInformation($"Returning success response for Department ID: {result.Id}");

            return new ApiResponseDTO<PwdRuleDto>
            {
                IsSuccess = true,
                Message = "Password Complexity Rule created successfully",
                Data = ruleDto
            };
        }

    }
}