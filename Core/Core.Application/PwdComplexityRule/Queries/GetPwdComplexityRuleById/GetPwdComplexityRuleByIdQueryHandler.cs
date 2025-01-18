using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRuleById
{
    public class GetPwdComplexityRuleByIdQueryHandler :IRequestHandler<GetPwdComplexityRuleByIdQuery, Result<PwdRuleDto>>
    {
         private readonly IPasswordComplexityRuleQueryRepository _pwdComplexityRuleQueryRepository;        
        private readonly IMapper _mapper;
         private readonly IMediator _mediator;


           public GetPwdComplexityRuleByIdQueryHandler(IPasswordComplexityRuleQueryRepository pwdComplexityRuleQueryRepository,IMapper mapper , IMediator mediator)
         {
            _pwdComplexityRuleQueryRepository = pwdComplexityRuleQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
        } 

        public async Task<Result<PwdRuleDto>> Handle(GetPwdComplexityRuleByIdQuery request, CancellationToken cancellationToken)
        {
                 var pwdcomplexityrule = await _pwdComplexityRuleQueryRepository.GetByIdAsync(request.Id);
                if (pwdcomplexityrule == null)
                {
                    return Result<PwdRuleDto>.Failure($"PasswordComplexityRule with ID {request.Id} not found.");
                }
                
                var PwdDto = _mapper.Map<PwdRuleDto>(pwdcomplexityrule);
                  
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: PwdDto.Id.ToString(),        
                    actionName: PwdDto.PwdComplexityRule,                
                    details: $"PasswordComplexityRule '{PwdDto.PwdComplexityRule}' was created. PasswordComplexityRule ID: {PwdDto.Id}",
                    module:"PasswordComplexityRule"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<PwdRuleDto>.Success(PwdDto);

            //   var user = await _departmentRepository.GetByIdAsync(request.DepartmentId);
            // return _mapper.Map<DepartmentDto>(user);

        }
    }
}