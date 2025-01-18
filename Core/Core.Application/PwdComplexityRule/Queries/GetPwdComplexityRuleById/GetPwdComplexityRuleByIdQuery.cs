using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using MediatR;

namespace Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRuleById
{
    public class GetPwdComplexityRuleByIdQuery :IRequest<Result<PwdRuleDto>>
    {
         public int Id { get; set; }
    }
}