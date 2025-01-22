using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.PwdComplexityRule.Queries.GetPwdComplexityRuleAutoComplete
{
    public class GetPwdComplexityRuleAutoComplete : IRequest<ApiResponseDTO<List<PwdRuleDto>>> 
    {
                public string SearchTerm  { get; set; } 
    }
}