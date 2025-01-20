using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRuleById
{
    public class GetPwdComplexityRuleByIdQueryHandler :IRequestHandler<GetPwdComplexityRuleByIdQuery, ApiResponseDTO<PwdRuleDto>>
    {
         private readonly IPasswordComplexityRuleQueryRepository _pwdComplexityRuleQueryRepository;        
        private readonly IMapper _mapper;
         private readonly IMediator _mediator;
        private readonly ILogger<GetPwdComplexityRuleByIdQueryHandler> _logger;


           public GetPwdComplexityRuleByIdQueryHandler(IPasswordComplexityRuleQueryRepository pwdComplexityRuleQueryRepository,IMapper mapper , IMediator mediator ,ILogger<GetPwdComplexityRuleByIdQueryHandler> logger)
         {
            _pwdComplexityRuleQueryRepository = pwdComplexityRuleQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
             _logger = logger;
        } 

        public async Task<ApiResponseDTO<PwdRuleDto>> Handle(GetPwdComplexityRuleByIdQuery request, CancellationToken cancellationToken)
        {

             _logger.LogInformation("Handling GetPwdComplexityRuleByIdQuery for ID: {Id}", request.Id);

              // Fetch password complexity rule by ID
                var pwdComplexityRule = await _pwdComplexityRuleQueryRepository.GetByIdAsync(request.Id);

                if (pwdComplexityRule == null)
                {
                    _logger.LogWarning("PasswordComplexityRule with ID {Id} not found.", request.Id);
                      return new ApiResponseDTO<PwdRuleDto>
                        {
                            IsSuccess = false,
                            Message = "Department not found."
                            
                        };

                 
                }

                _logger.LogInformation("PasswordComplexityRule with ID {Id} retrieved successfully.", request.Id);

                // Map the rule to DTO
                var pwdDto = _mapper.Map<PwdRuleDto>(pwdComplexityRule);

                // Publish domain event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: pwdDto.Id.ToString(),
                    actionName: pwdDto.PwdComplexityRule,
                    details: $"PasswordComplexityRule '{pwdDto.PwdComplexityRule}' was retrieved. PasswordComplexityRule ID: {pwdDto.Id}",
                    module: "PasswordComplexityRule"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
                _logger.LogInformation("Domain event published for PasswordComplexityRule with ID {Id}.", pwdDto.Id);
               
            return new ApiResponseDTO<PwdRuleDto> { IsSuccess = true, Message = "Success", Data = pwdDto };
           

        }
    }
}