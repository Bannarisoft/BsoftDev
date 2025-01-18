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

namespace Core.Application.PwdComplexityRule.Commands.DeletePasswordComplexityRule
{
    public class DeletePasswordComplexityRuleCommandHandler :IRequestHandler<DeletePasswordComplexityRuleCommand,Result<int>>
    {
        private readonly  IPasswordComplexityRuleCommandRepository _IpasswordComplexityRepository;  
       private readonly IMapper _Imapper;
           private readonly IMediator _mediator; 
       
        public DeletePasswordComplexityRuleCommandHandler (IPasswordComplexityRuleCommandRepository passwordcomplexityrulerepository , IMapper mapper)
      {
         _IpasswordComplexityRepository = passwordcomplexityrulerepository;
            _Imapper = mapper;
      }

       public async Task<Result<int>>Handle(DeletePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
      {       

     
        var pwdcomplexityrule = _Imapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request.updatePwdRuleStatusDto);
  
        var result = await _IpasswordComplexityRepository.DeleteAsync(request.Id, pwdcomplexityrule);
    
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Delete",
            actionCode: request.Id.ToString(),
            actionName:"",
            details:$"Admin Security settings Id: {request.Id} was Changed to Status Inactive.",
            module:"Admin Security settings"
        );


        await _mediator.Publish(domainEvent, cancellationToken);
         return Result<int>.Success(result);
         
      }
    }
}