using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRule;
using MediatR;

namespace Core.Application.PwdComplexityRule.Commands.DeletePasswordComplexityRule
{
    public class DeletePasswordComplexityRuleCommand  :IRequest<Result<int>>
    {

          public int Id { get; set; }
         public PwdRuleStatusDto updatePwdRuleStatusDto { get; set; }
         
    }
}