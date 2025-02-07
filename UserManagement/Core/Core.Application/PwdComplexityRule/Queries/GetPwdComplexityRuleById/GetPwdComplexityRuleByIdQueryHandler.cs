using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRule;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRuleById
{
    public class GetPwdComplexityRuleByIdQueryHandler :IRequestHandler<GetPwdComplexityRuleByIdQuery, ApiResponseDTO<GetPwdRuleDto>>
    {
         private readonly IPasswordComplexityRuleQueryRepository _pwdComplexityRuleQueryRepository;        
        private readonly IMapper _mapper;
         private readonly IMediator _mediator;
         private readonly IPasswordComplexityRuleCommandRepository _passwordComplexityRuleCommandRepository;

        private readonly ILogger<GetPwdComplexityRuleByIdQueryHandler> _logger;


           public GetPwdComplexityRuleByIdQueryHandler(IPasswordComplexityRuleCommandRepository passwordComplexityRuleCommandRepository,IPasswordComplexityRuleQueryRepository pwdComplexityRuleQueryRepository,IMapper mapper , IMediator mediator ,ILogger<GetPwdComplexityRuleByIdQueryHandler> logger)
         {
            _pwdComplexityRuleQueryRepository = pwdComplexityRuleQueryRepository;
            _passwordComplexityRuleCommandRepository=passwordComplexityRuleCommandRepository;
            _mapper =mapper;
            _mediator = mediator;
             _logger = logger;
        } 

        public async Task<ApiResponseDTO<GetPwdRuleDto>> Handle(GetPwdComplexityRuleByIdQuery request, CancellationToken cancellationToken)
        {

             _logger.LogInformation($"Handling GetPwdComplexityRuleByIdQuery for ID: {request.Id}");

              // Fetch password complexity rule by ID
                var pwdComplexityRule = await _pwdComplexityRuleQueryRepository.GetByIdAsync(request.Id);

                if (pwdComplexityRule is null)
                {
                    _logger.LogWarning($"PasswordComplexityRule with ID { request.Id} not found.");
                      return new ApiResponseDTO<GetPwdRuleDto>
                        {
                            IsSuccess = false,
                            Message = "PasswordComplexityRule not found / Deleted.",
                            Data= null                         
                        };                 
                }

                _logger.LogInformation($"PasswordComplexityRule with ID { request.Id} retrieved successfully.");

                // Map the rule to DTO
                var updatedpwdcomplexrule = _mapper.Map<GetPwdRuleDto>(pwdComplexityRule);
                
                // Publish domain event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: updatedpwdcomplexrule.Id.ToString(),
                    actionName: updatedpwdcomplexrule.PwdComplexityRule,
                    details: $"PasswordComplexityRule '{updatedpwdcomplexrule.PwdComplexityRule}' was retrieved. PasswordComplexityRule ID: {updatedpwdcomplexrule.Id}",
                    module: "PasswordComplexityRule"
                );


                await _mediator.Publish(domainEvent, cancellationToken);
                _logger.LogInformation($"Domain event published for PasswordComplexityRule with ID {updatedpwdcomplexrule.Id}." );
               
            return new ApiResponseDTO<GetPwdRuleDto> { IsSuccess = true, Message = "Success", Data = updatedpwdcomplexrule };           
        }
    }
}