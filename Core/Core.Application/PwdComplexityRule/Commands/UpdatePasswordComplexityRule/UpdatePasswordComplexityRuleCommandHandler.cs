using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using Core.Application.PwdComplexityRule.Queries;


namespace Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule
{
    public class UpdatePasswordComplexityRuleCommandHandler :IRequestHandler<UpdatePasswordComplexityRuleCommand, PwdRuleDto>
    {
         public readonly IPasswordComplexityRepository  _IPasswordComplexityRepository;
         private readonly IMapper _Imapper;
         
          public UpdatePasswordComplexityRuleCommandHandler(IPasswordComplexityRepository passwordComplexityRepository, IMapper mapper)
          {
              _IPasswordComplexityRepository = passwordComplexityRepository;
              _Imapper = mapper;

          }

        public async Task<PwdRuleDto> Handle(UpdatePasswordComplexityRuleCommand request, CancellationToken cancellationToken)
            {

                
              var passwordcomplexityrule = _Imapper.Map<Core.Domain.Entities.PasswordComplexityRule>(request);
               await _IPasswordComplexityRepository.UpdateAsync(request.Id, passwordcomplexityrule);
                 var dpwcomplexDto = _Imapper.Map<PwdRuleDto>(passwordcomplexityrule);
                    
               return dpwcomplexDto;

            }


 
        
    }
}