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
        private readonly ILogger<DeletePasswordComplexityRuleCommandHandler> _logger;
       
        public DeletePasswordComplexityRuleCommandHandler (IPasswordComplexityRuleCommandRepository passwordcomplexityrulerepository , IMapper mapper,ILogger<DeletePasswordComplexityRuleCommandHandler> logger)
      {
         _IpasswordComplexityRepository = passwordcomplexityrulerepository;
            _Imapper = mapper;
            _logger = logger;
      }

       public async Task<ApiResponseDTO<int>>Handle(DeletePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
      {       
        _logger.LogInformation("Handling DeletePasswordComplexityRuleCommand for Password Complexity Rule with ID: {Id}", request.Id);
          // Map the request to the entity
                var pwdComplexityRule = _Imapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request.updatePwdRuleStatusDto);

                // Perform the delete operation
                var result = await _IpasswordComplexityRepository.DeleteAsync(request.Id, pwdComplexityRule);

                if (result == 0)
                {
                    _logger.LogWarning("Failed to delete Password Complexity Rule with ID: {Id}", request.Id);
                
                      return new ApiResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "Department not found"
                     
                    };
                }

               _logger.LogInformation("Password Complexity Rule with ID {DepartmentId} found. Proceeding with deletion.", request.Id);
                 // Map request to entity and delete
            var updatedpasswordComplexity = _Imapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request.updatePwdRuleStatusDto);
            var pwdcompresult = await _IpasswordComplexityRepository.DeleteAsync(request.Id, updatedpasswordComplexity);

            if (pwdcompresult <= 0)
            {
                _logger.LogWarning("Failed to delete Department with ID {DepartmentId}.", request.Id);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to delete department",
                    Data = pwdcompresult
                };
            }

                _logger.LogInformation("Password Complexity Rule with ID: {Id} deleted successfully.", request.Id);

                // Publish domain event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: request.Id.ToString(),
                    actionName: "",
                    details: $"Admin Security settings ID: {request.Id} was changed to Status Inactive.",
                    module: "Admin Security settings"
                );

                 await _mediator.Publish(domainEvent, cancellationToken);
                _logger.LogInformation("AuditLogsDomainEvent published for  Password Complexity Rule ID {Id}.", request.Id);

                return new ApiResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = " Password Complexity Rule deleted successfully"
                    
                };  
                
     
       
         
      }
    }
}