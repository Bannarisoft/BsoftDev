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
       
        public DeletePasswordComplexityRuleCommandHandler (IPasswordComplexityRuleCommandRepository passwordcomplexityrulerepository,IPasswordComplexityRuleQueryRepository passwordComplexityRuleQueryRepository , IMapper mapper,ILogger<DeletePasswordComplexityRuleCommandHandler> logger)
      {
         _IpasswordComplexityRepository = passwordcomplexityrulerepository;
            _Imapper = mapper;
            _logger = logger;
      }
       public async Task<ApiResponseDTO<int>>Handle(DeletePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
      {       
        _logger.LogInformation($"Handling DeletePasswordComplexityRuleCommand for Password Complexity Rule with ID: { request.Id}");

          var PasswordComplexityRulemap = _Imapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request);

             _logger.LogInformation($"PasswordComplexityRule  with ID {request.Id} found. Proceeding with deletion." );

             var userrole = await _IpasswordComplexityRepository.DeleteAsync(request.Id, PasswordComplexityRulemap);
            
               _logger.LogInformation($"Password Complexity Rule with ID { request.Id} found. Proceeding with deletion.");
                 // Map request to entity and delete
            var updatedpasswordComplexity = _Imapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request);
           
            var pwdcompresult = await _IpasswordComplexityRepository.DeleteAsync(request.Id, updatedpasswordComplexity);

            if (pwdcompresult <= 0)
            {
                _logger.LogWarning($"Failed to delete Password Complexity  with ID {request.Id}." );
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to delete Password Complexity / Deleted"                   
                };
            }
                _logger.LogInformation($"Password Complexity Rule with ID: {request.Id} deleted successfully.");

                // Publish domain event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: updatedpasswordComplexity.Id.ToString(),
                    actionName: "",
                    details: $" Password Complexitys ID: {request.Id} was changed to Status Inactive.",
                    module: " Password Complexity"
                );

                 await _mediator.Publish(domainEvent, cancellationToken);
                _logger.LogInformation($"AuditLogsDomainEvent published for  Password Complexity Rule ID {request.Id}." );

                return new ApiResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = " Password Complexity Rule deleted successfully"
                   
                    
                };                              
         
      }
    }
}