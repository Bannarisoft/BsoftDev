using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using Core.Application.PwdComplexityRule.Queries;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using Core.Application.Common;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;
using Core.Application.Common.HttpResponse;
using Core.Application.UserRole.Queries.GetRole;


namespace Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRuleAutoComplete
{
    public class GetPwdComplexityRuleAutoCompleteHandler : IRequestHandler<GetPwdComplexityRuleAutoComplete, ApiResponseDTO<List<PwdComplexityRuleAutoCompleteDto>>>
    {              
        private readonly IPasswordComplexityRuleCommandRepository _passwordComplexityRuleCommandRepository;
        private readonly IPasswordComplexityRuleQueryRepository _passwordComplexityRuleQueryRepository;             
        private readonly IMapper _mapper;
        private readonly ILogger<GetPwdComplexityRuleAutoCompleteHandler> _logger;
        private readonly IMediator _mediator;


    public GetPwdComplexityRuleAutoCompleteHandler(IPasswordComplexityRuleCommandRepository passwordComplexityRuleCommandRepository,IPasswordComplexityRuleQueryRepository passwordComplexityRuleQueryRepository , IMapper mapper, IMediator mediator,ILogger<GetPwdComplexityRuleAutoCompleteHandler> logger)
        {
        _passwordComplexityRuleCommandRepository=passwordComplexityRuleCommandRepository;
        _passwordComplexityRuleQueryRepository=passwordComplexityRuleQueryRepository;
            _mapper =mapper;
            _mediator=mediator;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<List<PwdComplexityRuleAutoCompleteDto>>> Handle(GetPwdComplexityRuleAutoComplete request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling GetPwdComplexityRuleAutoCompleteQuery with search pattern: {request.SearchTerm}");

            // Fetch password complexity rules matching the search pattern
            var pwdComplexityRules = await _passwordComplexityRuleQueryRepository.GetpwdautocompleteAsync(request.SearchTerm);

            // if (pwdComplexityRules is null || !pwdComplexityRules.Any())
            // {
            //     _logger.LogWarning($"No password complexity rules found for search pattern: {request.SearchTerm}");
            //     return new ApiResponseDTO<List<PwdComplexityRuleAutoCompleteDto>>
            //     {
            //         IsSuccess = false,
            //         Message = "No matching password complexity rules found",
            //         Data = new List<PwdComplexityRuleAutoCompleteDto>()
            //     };
            // }

            _logger.LogInformation($"Password complexity rules found for search pattern: {request.SearchTerm}. Mapping results to DTO.");

            // Map the result to DTO
            var pwdRuleDtoList = _mapper.Map<List<PwdComplexityRuleAutoCompleteDto>>(pwdComplexityRules);

            // Publish domain event for audit logs
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutoComplete",
                actionCode: "",
                actionName: request.SearchTerm,
                details: $"Password Complexity Rule '{request.SearchTerm}' was searched",
                module: "Password Complexity Rule"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            _logger.LogInformation($"Domain event published for search pattern: {request.SearchTerm}");

            return new ApiResponseDTO<List<PwdComplexityRuleAutoCompleteDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = pwdRuleDtoList
            };
        }

        //  public async Task<ApiResponseDTO<List<PwdComplexityRuleAutoCompleteDto>>> Handle(GetPwdComplexityRuleAutoComplete request, CancellationToken cancellationToken)
        // {

        //           _logger.LogInformation($"Handling GetPwdComplexityRuleAutoCompleteSearchQuery with search pattern: {request.SearchTerm}");

        //      // Fetch PwdComplexityRule matching the search pattern
        //         var result = await _passwordComplexityRuleQueryRepository.GetpwdautocompleteAsync(request.SearchTerm);
        //         // if (!result.Any() || result.Count() == 0)
        //         // {
        //         //     _logger.LogWarning($"No Password Password Rule  found for search pattern: { request.SearchTerm}");
        //         //     return new ApiResponseDTO<List<PwdComplexityRuleAutoCompleteDto>>
        //         //     {
        //         //         IsSuccess = false,
        //         //         Message = "No matching Password Password Rule  found",
        //         //         Data = new List<PwdComplexityRuleAutoCompleteDto>()
        //         //     };
        //         // }

        //         _logger.LogInformation($"Password  Rule  found for search pattern: { request.SearchTerm}. Mapping results to DTO.");

        //         // Map the result to DTO
        //         var pwdRoleDto = _mapper.Map<List<PwdComplexityRuleAutoCompleteDto>>(result);

        //         // Publish domain event for audit logs
        //         var domainEvent = new AuditLogsDomainEvent(
        //             actionDetail: "GetAutoComplete",
        //             actionCode: "",
        //             actionName: request.SearchTerm,
        //             details: $"Password Complexity Rule'{request.SearchTerm}' was searched",
        //             module: "Password Complexity Rule"
        //         );
        //         await _mediator.Publish(domainEvent, cancellationToken);

        //         _logger.LogInformation($"Domain event published for search pattern: { request.SearchTerm}");

        //         return new ApiResponseDTO<List<PwdComplexityRuleAutoCompleteDto>>
        //         {
        //             IsSuccess = true,
        //             Message = "Success",
        //             Data=pwdRoleDto                    
        //         };                   
        // }


    }
}