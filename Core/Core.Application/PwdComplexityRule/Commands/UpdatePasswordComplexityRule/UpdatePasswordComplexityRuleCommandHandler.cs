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
using Core.Domain.Enums.Common;


namespace Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule
{
    public class UpdatePasswordComplexityRuleCommandHandler :IRequestHandler<UpdatePasswordComplexityRuleCommand, ApiResponseDTO<PwdRuleDto>>
    {
         public readonly IPasswordComplexityRuleCommandRepository  _IPasswordComplexityRepository;
         private readonly IMapper _Imapper;  
        private readonly IPasswordComplexityRuleQueryRepository _IpasswordComplexityRuleQueryRepository;
        private readonly IMediator _mediator; 
            private readonly ILogger<UpdatePasswordComplexityRuleCommandHandler> _logger;
         
          public UpdatePasswordComplexityRuleCommandHandler(IPasswordComplexityRuleCommandRepository passwordComplexityRepository,IPasswordComplexityRuleQueryRepository passwordComplexityRuleQueryRepository, IMapper mapper,IMediator mediator,ILogger<UpdatePasswordComplexityRuleCommandHandler> logger)
          {
              _IPasswordComplexityRepository = passwordComplexityRepository;
             
            _IpasswordComplexityRuleQueryRepository = passwordComplexityRuleQueryRepository;
              _Imapper = mapper;
              _mediator = mediator;
              _logger = logger;

          }

        public async Task<ApiResponseDTO<PwdRuleDto>> Handle(UpdatePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
            {
              _logger.LogInformation("Handling UpdatePasswordComplexityRuleCommand for Password Complexity Rule with ID: {Id}", request.Id);


                 var dePasswordComplexityMap  = _Imapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request);
                    // // Retrieve the password complexity rule by ID
                    // var pwdComplexity = await _IpasswordComplexityRuleQueryRepository.GetByIdAsync(request.Id , dePasswordComplexityMap);
                    //  // Fetch the department by ID
                       
                    //     if (pwdComplexity == null)
                    //     {
                    //         _logger.LogWarning("Password Complexity Rule  with ID {Id} not found.", request.Id);
                    //         return new ApiResponseDTO<PwdRuleDto>
                    //         {
                    //             IsSuccess = false,
                    //             Message = "Password Complexity Rule  not found"
                    //         };
                    //     }

                    _logger.LogInformation("Password Complexity Rule with ID {Id} retrieved successfully. Proceeding with update.", request.Id);

                    // Update the properties
                    // dePasswordComplexityMap.PwdComplexityRule = request.PwdComplexityRule;
                    // dePasswordComplexityMap.IsActive=request.IsActive;
                   

                    // Save the updates
                    var result = await _IPasswordComplexityRepository.UpdateAsync(request.Id, dePasswordComplexityMap);
                                if (result == null)
                        {
                            _logger.LogWarning("Failed to update Password Complexity Rule with ID {Id}.", request.Id);
                            return new ApiResponseDTO<PwdRuleDto>
                            {
                                IsSuccess = false,
                                Message = "Failed to update Password Complexity Rule"
                            };
                        }
                  

                    _logger.LogInformation("Password Complexity Rule with ID {Id} updated successfully.", request.Id);

                    // Publish domain event
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: dePasswordComplexityMap.Id.ToString(),
                        actionName: dePasswordComplexityMap.PwdComplexityRule,
                        details: $"Password Complexity Rule '{dePasswordComplexityMap.PwdComplexityRule}' was updated. Password Complexity Rule ID: {request.Id}",
                        module: "PasswordComplexityRule"
                    );

                 
                      await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for Password Complexity Rule ID {DepartmentId}.", request.Id);

            return new ApiResponseDTO<PwdRuleDto>
            {
                IsSuccess = true,
                Message = "Password Complexity Rule updated successfully"
               
            };                                     

            }


 
        
    }
}