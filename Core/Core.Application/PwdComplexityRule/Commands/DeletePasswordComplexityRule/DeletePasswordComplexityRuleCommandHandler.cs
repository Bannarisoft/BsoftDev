using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using MediatR;
using AutoMapper;
using Core.Domain.Entities;

namespace Core.Application.PwdComplexityRule.Commands.DeletePasswordComplexityRule
{
    public class DeletePasswordComplexityRuleCommandHandler :IRequestHandler<DeletePasswordComplexityRuleCommand,int>
    {
        private readonly  IPasswordComplexityRepository _IpasswordComplexityRepository;  
       private readonly IMapper _mapper;
        public DeletePasswordComplexityRuleCommandHandler (IPasswordComplexityRepository passwordcomplexityrulerepository , IMapper mapper)
      {
         _IpasswordComplexityRepository = passwordcomplexityrulerepository;
            _mapper = mapper;
      }

       public async Task<int>Handle(DeletePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
      {       
         var updatedpwdrule = _mapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request.updatePwdRuleStatusDto);    
          await _IpasswordComplexityRepository.DeleteAsync(request.Id , updatedpwdrule); 
          return updatedpwdrule.Id;
         
      }
    }
}