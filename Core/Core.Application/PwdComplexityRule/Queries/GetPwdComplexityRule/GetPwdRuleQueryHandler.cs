using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;

namespace Core.Application.PwdComplexityRule.Queries
{
    public class GetPwdRuleQueryHandler  :IRequestHandler<GetPwdRuleQuery, List<PwdRuleDto>>
    {
       private readonly IPasswordComplexityRuleQueryRepository _passwordComplexityRepository; 
       private readonly IMapper _mapper; 
      


        public GetPwdRuleQueryHandler( IPasswordComplexityRuleQueryRepository passwordComplexityRepository,IMapper mapper)
        {
             _mapper =mapper;
            _passwordComplexityRepository = passwordComplexityRepository;   
        }

        public async Task<List<PwdRuleDto>> Handle(GetPwdRuleQuery request, CancellationToken cancellationToken)
        {

          var   pwdComplexityRules = await _passwordComplexityRepository.GetPasswordComplexityAsync();
            return _mapper.Map<List<PwdRuleDto>>(pwdComplexityRules);

     

        }
    }

    

}