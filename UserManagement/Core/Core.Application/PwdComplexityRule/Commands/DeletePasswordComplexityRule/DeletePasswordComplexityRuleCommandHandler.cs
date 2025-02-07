using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using MediatR;
using AutoMapper;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using Core.Application.Common;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;
using Core.Application.Common.HttpResponse;

namespace Core.Application.PwdComplexityRule.Commands.DeletePasswordComplexityRule
{
    public class DeletePasswordComplexityRuleCommandHandler :IRequestHandler<DeletePasswordComplexityRuleCommand,ApiResponseDTO<int>>
    {
        private readonly  IPasswordComplexityRuleCommandRepository _IpasswordComplexityRepository;  
       private readonly IMapper _Imapper;
        private readonly IMediator _mediator; 
        private readonly IPasswordComplexityRuleQueryRepository _passwordComplexityRuleQueryRepository;
        private readonly ILogger<DeletePasswordComplexityRuleCommandHandler> _logger;
       
        public DeletePasswordComplexityRuleCommandHandler (IPasswordComplexityRuleCommandRepository passwordcomplexityrulerepository,IPasswordComplexityRuleQueryRepository passwordComplexityRuleQueryRepository , IMapper mapper,ILogger<DeletePasswordComplexityRuleCommandHandler> logger,IMediator mediator)
      {
         _IpasswordComplexityRepository = passwordcomplexityrulerepository;
            _Imapper = mapper;
            _logger = logger;
            _mediator = mediator;

      }
       public async Task<ApiResponseDTO<int>>Handle(DeletePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
      {       
   
            
             // Map the Delete command to the entity
    var passwordComplexityRuleMap = _Imapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request);

    _logger.LogInformation($"PasswordComplexityRule with ID {request.Id} found. Proceeding with deletion.");

    // Call the repository to delete the Password Complexity Rule
    var pwdcompresult = await _IpasswordComplexityRepository.DeleteAsync(request.Id, passwordComplexityRuleMap);

    // Check if the deletion was successful
    if (pwdcompresult <= 0)
    {
        _logger.LogWarning("Failed to delete Password Complexity Rule with ID {PasswordComplexityRuleId}.", request.Id);
        return new ApiResponseDTO<int>
        {
            IsSuccess = false,
            Message = "Failed to delete Password Complexity Rule",
            Data = 0
        };
    }

    _logger.LogInformation("Password Complexity Rule with ID {PasswordComplexityRuleId} deleted successfully.", request.Id);

    // Publish the domain event for audit logs
    var domainEvent = new AuditLogsDomainEvent(
        actionDetail: "Delete",
        actionCode: passwordComplexityRuleMap.PwdComplexityRule,
        actionName: "",
        details: $"Password Complexity Rule ID: {request.Id} was changed to status inactive.",
        module: "Password Complexity"
    );

    await _mediator.Publish(domainEvent, cancellationToken);
    _logger.LogInformation($"AuditLogsDomainEvent published for Password Complexity Rule ID {request.Id}." );

    return new ApiResponseDTO<int>
    {
        IsSuccess = true,
        Message = "Password Complexity Rule deleted successfully",
        Data = 0
    };                         
         
      }
    }
}