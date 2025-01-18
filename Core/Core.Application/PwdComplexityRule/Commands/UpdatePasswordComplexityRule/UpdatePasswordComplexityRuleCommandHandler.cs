using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using Core.Application.PwdComplexityRule.Queries;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using Core.Application.Common;
using Core.Domain.Events;


namespace Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule
{
    public class UpdatePasswordComplexityRuleCommandHandler :IRequestHandler<UpdatePasswordComplexityRuleCommand, Result<int>>
    {
         public readonly IPasswordComplexityRuleCommandRepository  _IPasswordComplexityRepository;
         private readonly IMapper _Imapper;  
                 private readonly IPasswordComplexityRuleQueryRepository _IpasswordComplexityRuleQueryRepository;
        private readonly IMediator _mediator; 
         
          public UpdatePasswordComplexityRuleCommandHandler(IPasswordComplexityRuleCommandRepository passwordComplexityRepository,IPasswordComplexityRuleQueryRepository passwordComplexityRuleQueryRepository, IMapper mapper,IMediator mediator)
          {
              _IPasswordComplexityRepository = passwordComplexityRepository;
             
            _IpasswordComplexityRuleQueryRepository = passwordComplexityRuleQueryRepository;
              _Imapper = mapper;
              _mediator = mediator;

          }

        public async Task<Result<int>> Handle(UpdatePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
            {

                  var pwdcomplexity = await _IpasswordComplexityRuleQueryRepository.GetByIdAsync(request.Id);
                 
                  pwdcomplexity.PwdComplexityRule = request.PwdComplexityRule; 
                  pwdcomplexity.IsActive = request.IsActive;
                 
                  var result = await _IPasswordComplexityRepository.UpdateAsync(request.Id, pwdcomplexity);
                  if (result == 0) 
                  {
                      return Result<int>.Failure("Failed to update the PasswordComplexityRule.");
                  }                 
                  var domainEvent = new AuditLogsDomainEvent(
                      actionDetail: "Update",
                      actionCode: pwdcomplexity.Id.ToString(),
                      actionName: pwdcomplexity.PwdComplexityRule,
                      details: $"PasswordComplexityRule '{pwdcomplexity.PwdComplexityRule}' was updated. PasswordComplexityRule ID: {request.Id}",
                      module: "PasswordComplexityRule"
                  );
                  await _mediator.Publish(domainEvent, cancellationToken);
                  
                  return Result<int>.Success(result);

            }


 
        
    }
}