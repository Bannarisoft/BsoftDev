using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRules.Queries.GetApprovalRuleAutoComplete
{
    public class GetApprovalRuleAutoCompleteQuery : IRequest<List<ApprovalRuleAutoCompleteDto>>
    {
        public string? SearchPattern { get; set; } 
    }
}